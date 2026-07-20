using System.Text.Json.Serialization;
using RestaurantApi.Dtos;
using RestaurantApi.Enums;
using RestaurantApi.Factories;
using RestaurantApi.Interfaces;
using RestaurantApi.Models;
using RestaurantApi.Repositories;
using RestaurantApi.Services;
using RestaurantApi.Singletons;
using RestaurantApi.Strategies;

var builder = WebApplication.CreateBuilder(args);

// Serialize enums as strings in JSON responses
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<OrderRepository>();
builder.Services.AddSingleton<DishFactoryProvider>();
builder.Services.AddSingleton<PriceCalculator>();

builder.Services.AddSingleton<INotificationObserver, KitchenService>();
builder.Services.AddSingleton<INotificationObserver, DiningService>();
builder.Services.AddSingleton<INotificationObserver, BillingService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var repository = app.Services.GetRequiredService<OrderRepository>();

Console.WriteLine($"[Startup] {RestaurantConfiguration.Instance.RestaurantName}");

app.MapGet("/", () => "Restaurant API is running. See /swagger for details.");

app.MapPost(
    "/api/dishes",
    (CreateDishRequest request, DishFactoryProvider provider) =>
    {
        var factory = provider.GetFactory(request.Category);
        IDish dish = factory.CreateDish(request);

        return Results.Ok(
            new
            {
                dish.Name,
                dish.Price,
                dish.Category,
                dish.Description,
                PreparationTimeMinutes = dish.PreparationTime.TotalMinutes,
            }
        );
    }
);

app.MapPatch(
    "/api/orders/{id}/policy",
    (string id, PricingPolicy policy, PriceCalculator calculator) =>
    {
        var order = repository.GetById(id);
        if (order is null)
            return Results.NotFound();

        order.Policy = policy;
        order.TotalPrice = calculator.CalculatePrice(order);

        return Results.Ok(new { order.TotalPrice, Description = calculator.GetDescription(order) });
    }
);

// POST /api/orders : creates an order.
// Dishes are instantiated by factories (Factory), and the total is calculated
// by the strategy corresponding to the pricing policy,  (Strategy).
app.MapPost(
    "/api/orders",
    (
        CreateOrderRequest request,
        DishFactoryProvider provider,
        PriceCalculator calculator,
        IEnumerable<INotificationObserver> observers
    ) =>
    {
        if (request.Items is null || request.Items.Count == 0)
            return Results.BadRequest(
                new { Error = "La commande doit contenir au moins un plat." }
            );

        var order = new Order { TableNumber = request.TableNumber, Policy = request.Policy };

        // Subscription before any notification: the constructor cannot notify,
        // no observer is yet subscribed.
        foreach (var observer in observers)
        {
            order.Subscribe(observer);
        }

        foreach (var item in request.Items)
        {
            order.AddItem(provider.GetFactory(item.Category).CreateDish(item));
        }

        order.TotalPrice = calculator.CalculatePrice(order);
        repository.Add(order);

        // Creation event: the order is in ReceivedState and the total is
        // calculated, so Billing receives the correct amount.
        order.Notify(order);

        return Results.Created(
            $"/api/orders/{order.Id}",
            OrderResponse.From(order, calculator.GetDescription(order))
        );
    }
);

// GET /api/orders : lists all orders.
app.MapGet(
    "/api/orders",
    (PriceCalculator calculator) =>
        Results.Ok(
            repository
                .GetAll()
                .Select(order => OrderResponse.From(order, calculator.GetDescription(order)))
        )
);

// GET /api/orders/{id} : gets a specific order.
app.MapGet(
    "/api/orders/{id}",
    (string id, PriceCalculator calculator) =>
    {
        var order = repository.GetById(id);
        if (order is null)
            return Results.NotFound();

        return Results.Ok(OrderResponse.From(order, calculator.GetDescription(order)));
    }
);

// PUT /api/orders/{id}/state : advances the order in its workflow.
// Received -> Preparation -> Ready -> Served -> Paid (State).
app.MapPut(
    "/api/orders/{id}/state",
    (string id, PriceCalculator calculator) =>
    {
        var order = repository.GetById(id);
        if (order is null)
            return Results.NotFound();

        try
        {
            order.Status.NextState();
        }
        catch (InvalidOperationException ex)
        {
            return Results.Conflict(new { Error = ex.Message });
        }

        return Results.Ok(OrderResponse.From(order, calculator.GetDescription(order)));
    }
);

// GET /api/orders/{id}/observers : services currently subscribed to the order.
app.MapGet(
    "/api/orders/{id}/observers",
    (string id) =>
    {
        var order = repository.GetById(id);
        if (order is null)
            return Results.NotFound();

        return Results.Ok(order.GetObserverNames());
    }
);

// DELETE /api/orders/{id}/observers/{name} : unsubscribes a service (Kitchen, Dining,
// Billing). Subsequent transitions will no longer be sent to it.
app.MapDelete(
    "/api/orders/{id}/observers/{name}",
    (string id, string name) =>
    {
        var order = repository.GetById(id);
        if (order is null)
            return Results.NotFound();

        if (!order.UnsubscribeByName(name))
            return Results.NotFound(new { Error = $"No subscribed service named '{name}'." });

        Console.WriteLine($"[Order {order.Id[..8]}] Service '{name}' unsubscribed.");
        return Results.Ok(order.GetObserverNames());
    }
);

// GET /api/menu : complete menu, grouped by category.
// Read from the single shared instance (Singleton), not from DI.
app.MapGet(
    "/api/menu",
    () =>
        Results.Ok(
            RestaurantConfiguration
                .Instance.Menu.GroupBy(dish => dish.Category)
                .Select(group => new
                {
                    Category = group.Key,
                    Dishes = group.Select(DishResponse.From).ToList(),
                })
        )
);

// GET /api/restaurant : opening hours and global settings, same shared instance.
app.MapGet(
    "/api/restaurant",
    () =>
    {
        var config = RestaurantConfiguration.Instance;

        return Results.Ok(
            new
            {
                config.RestaurantName,
                config.Currency,
                config.TableCount,
                config.ServiceChargeRate,
                OpeningHours = config.OpeningHours.Select(hours => new
                {
                    Day = hours.Day.ToString(),
                    Opening = hours.Opening?.ToString("HH:mm"),
                    Closing = hours.Closing?.ToString("HH:mm"),
                    hours.IsClosed,
                }),
            }
        );
    }
);

app.Run();
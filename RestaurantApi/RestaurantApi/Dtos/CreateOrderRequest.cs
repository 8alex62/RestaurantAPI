using RestaurantApi.Enums;

namespace RestaurantApi.Dtos
{
    public record CreateOrderRequest(
        int TableNumber,
        List<CreateDishRequest> Items,
        PricingPolicy Policy = PricingPolicy.Standard
    );
}
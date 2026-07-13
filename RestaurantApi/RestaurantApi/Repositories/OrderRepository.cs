using RestaurantApi.Models;

namespace RestaurantApi.Repositories;

public class OrderRepository
{
    private readonly Dictionary<string, Order> _orders = new();

    public void Add(Order order)
    {
        _orders[order.Id] = order;
    }

    public Order? GetById(string id)
    {
        return _orders.TryGetValue(id, out var order) ? order : null;
    }

    public List<Order> GetAll()
    {
        return _orders.Values.ToList();
    }
}

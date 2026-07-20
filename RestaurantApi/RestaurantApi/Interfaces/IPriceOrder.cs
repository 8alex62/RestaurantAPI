using RestaurantApi.Models;

namespace RestaurantApi.Interfaces
{
    public interface IPriceOrder
    {
        double CalculatePrice(Order order);

        string GetDescription();
    }
}
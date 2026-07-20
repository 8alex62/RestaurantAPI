using RestaurantApi.Interfaces;
using RestaurantApi.Models;

namespace RestaurantApi.Strategies
{
    public class StandardStrategy : IPriceOrder
    {
        public double CalculatePrice(Order order)
        {
            return order.Items.Sum(dish => dish.Price);
        }

        public string GetDescription()
        {
            return "Standard pricing strategy: No discounts applied";
        }
    }
}
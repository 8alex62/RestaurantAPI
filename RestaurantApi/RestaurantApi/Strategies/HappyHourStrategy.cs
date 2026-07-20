using RestaurantApi.Interfaces;
using RestaurantApi.Models;

namespace RestaurantApi.Strategies
{
    public class HappyHourStrategy : IPriceOrder
    {
        public double CalculatePrice(Order order)
        {
            return order.Items.Sum(dish => dish.Price) * 0.8; // Apply 20% discount
        }

        public string GetDescription()
        {
            return "Happy Hour pricing strategy: 20% off all dishes";
        }
    }
}
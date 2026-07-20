using RestaurantApi.Interfaces;
using RestaurantApi.Models;

namespace RestaurantApi.Strategies
{
    public class GroupStrategy : IPriceOrder
    {
        public double CalculatePrice(Order order)
        {
            double totalPrice = order.Items.Sum(dish => dish.Price);
            if (totalPrice > 50)
            {
                totalPrice *= 0.85; // Apply 15% discount
            }
            return totalPrice;
        }

        public string GetDescription()
        {
            return "Group pricing strategy: 15% off for orders over $50";
        }
    }
}
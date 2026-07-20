using RestaurantApi.Enums;
using RestaurantApi.Interfaces;
using RestaurantApi.Models;

namespace RestaurantApi.Strategies
{
    public class MenuStrategy : IPriceOrder
    {
        private const double MenuPrice = 25;

        public double CalculatePrice(Order order)
        {
            var remaining = new List<IDish>(order.Items);
            double total = 0;

            while (TryExtractMenu(remaining))
            {
                total += MenuPrice;
            }

            return total + remaining.Sum(dish => dish.Price);
        }

        private static bool TryExtractMenu(List<IDish> remaining)
        {
            var starter = PickBest(remaining, DishCategory.Starter);
            var main = PickBest(remaining, DishCategory.MainCourse);
            var dessert = PickBest(remaining, DishCategory.Dessert);

            if (starter is null || main is null || dessert is null)
            {
                return false;
            }

            remaining.Remove(starter);
            remaining.Remove(main);
            remaining.Remove(dessert);
            return true;
        }

        private static IDish? PickBest(List<IDish> remaining, DishCategory category) =>
            remaining
                .Where(dish => dish.Category == category)
                .OrderByDescending(dish => dish.Price)
                .FirstOrDefault();

        public string GetDescription()
        {
            return $"Menu pricing strategy: Fixed price of {MenuPrice} for a complete menu (starter, main course, dessert)";
        }
    }
}
using RestaurantApi.Enums;
using RestaurantApi.Interfaces;

namespace RestaurantApi.Categories
{
    public class Dessert(string name, double price, TimeSpan preparationTime) : IDish
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; } = name;
        public double Price { get; } = price;
        public TimeSpan PreparationTime { get; } = preparationTime;
        public DishCategory Category => DishCategory.Dessert;

        public string Description =>
            $"Dessert: {Name}, served in {PreparationTime.TotalMinutes} min.";
    }
}
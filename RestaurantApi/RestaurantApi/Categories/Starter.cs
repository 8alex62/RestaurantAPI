using RestaurantApi.Enums;
using RestaurantApi.Interfaces;

namespace RestaurantApi.Categories
{
    public class Starter(string name, double price, TimeSpan preparationTime) : IDish
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; } = name;
        public double Price { get; } = price;
        public TimeSpan PreparationTime { get; } = preparationTime;
        public DishCategory Category => DishCategory.Starter;

        public string Description =>
            $"Starter: {Name}, served in {PreparationTime.TotalMinutes} min.";
    }
}
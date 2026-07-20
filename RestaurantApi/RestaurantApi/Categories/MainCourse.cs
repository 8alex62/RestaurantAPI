using RestaurantApi.Enums;
using RestaurantApi.Interfaces;

namespace RestaurantApi.Categories
{
    public class MainCourse(string name, double price, TimeSpan preparationTime) : IDish
    {
        public Guid Id { get; } = Guid.NewGuid();

        public string Description =>
            $"Main course: {Name}, served in {PreparationTime.TotalMinutes} min.";

        public string Name { get; } = name;

        public double Price { get; } = price;

        public TimeSpan PreparationTime { get; } = preparationTime;

        public DishCategory Category => DishCategory.MainCourse;
    }
}
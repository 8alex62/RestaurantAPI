using RestaurantApi.Enums;
using RestaurantApi.Interfaces;

namespace RestaurantApi.Categories
{
    public class Beverage(string name, double price, TimeSpan preparationTime, bool isAlcoholic)
        : IDish
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; } = name;
        public double Price { get; } = price;
        public TimeSpan PreparationTime { get; } = preparationTime;
        public DishCategory Category => DishCategory.Beverage;

        public bool IsAlcoholic { get; } = isAlcoholic;

        public string Description => $"Beverage{(IsAlcoholic ? " (alcoholic)" : "")}: {Name}.";
    }
}
using RestaurantApi.Enums;
using RestaurantApi.Factories;

namespace RestaurantApi.Interfaces
{
    public interface IDish
    {
        string Name { get; }

        double Price { get; }

        TimeSpan PreparationTime { get; }

        DishCategory Category { get; }

        string Description { get; }
    }
}
using RestaurantApi.Enums;
using RestaurantApi.Interfaces;

namespace RestaurantApi.Dtos
{
    public record DishResponse(
        string Name,
        double Price,
        DishCategory Category,
        string Description,
        double PreparationTimeMinutes
    )
    {
        public static DishResponse From(IDish dish) =>
            new(
                dish.Name,
                dish.Price,
                dish.Category,
                dish.Description,
                dish.PreparationTime.TotalMinutes
            );
    }
}

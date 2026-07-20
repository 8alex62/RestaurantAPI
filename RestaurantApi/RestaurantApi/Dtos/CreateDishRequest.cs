using RestaurantApi.Enums;

namespace RestaurantApi.Dtos
{
    public record CreateDishRequest(
        string Name,
        double Price,
        DishCategory Category,
        int? PreparationTimeMinutes = null,
        bool? IsAlcoholic = null
    );
}
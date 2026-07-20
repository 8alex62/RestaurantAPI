using RestaurantApi.Categories;
using RestaurantApi.Dtos;
using RestaurantApi.Interfaces;

namespace RestaurantApi.Factories
{
    public class BeverageFactory : DishFactory
    {
        protected override TimeSpan DefaultPreparationTime => TimeSpan.FromMinutes(2);

        public override IDish CreateDish(CreateDishRequest request)
        {
            return new Beverage(
                request.Name,
                request.Price,
                ResolvePreparationTime(request),
                request.IsAlcoholic ?? false
            );
        }
    }
}

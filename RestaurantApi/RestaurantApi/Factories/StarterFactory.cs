using RestaurantApi.Categories;
using RestaurantApi.Dtos;
using RestaurantApi.Interfaces;

namespace RestaurantApi.Factories
{
    public class StarterFactory : DishFactory
    {
        protected override TimeSpan DefaultPreparationTime => TimeSpan.FromMinutes(10);

        public override IDish CreateDish(CreateDishRequest request)
        {
            return new Starter(request.Name, request.Price, ResolvePreparationTime(request));
        }
    }
}

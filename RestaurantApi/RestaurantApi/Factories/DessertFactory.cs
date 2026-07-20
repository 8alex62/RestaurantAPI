using RestaurantApi.Categories;
using RestaurantApi.Dtos;
using RestaurantApi.Interfaces;

namespace RestaurantApi.Factories
{
    public class DessertFactory : DishFactory
    {
        protected override TimeSpan DefaultPreparationTime => TimeSpan.FromMinutes(8);

        public override IDish CreateDish(CreateDishRequest request)
        {
            return new Dessert(request.Name, request.Price, ResolvePreparationTime(request));
        }
    }
}

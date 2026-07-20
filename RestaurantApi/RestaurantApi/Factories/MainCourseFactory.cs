using RestaurantApi.Categories;
using RestaurantApi.Dtos;
using RestaurantApi.Interfaces;

namespace RestaurantApi.Factories
{
    public class MainCourseFactory : DishFactory
    {
        protected override TimeSpan DefaultPreparationTime => TimeSpan.FromMinutes(25);

        public override IDish CreateDish(CreateDishRequest request)
        {
            return new MainCourse(request.Name, request.Price, ResolvePreparationTime(request));
        }
    }
}

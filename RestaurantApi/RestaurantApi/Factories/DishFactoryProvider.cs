using RestaurantApi.Enums;

namespace RestaurantApi.Factories
{
    public class DishFactoryProvider
    {
        private readonly Dictionary<DishCategory, DishFactory> factories = new()
        {
            [DishCategory.Starter] = new StarterFactory(),
            [DishCategory.MainCourse] = new MainCourseFactory(),
            [DishCategory.Dessert] = new DessertFactory(),
            [DishCategory.Beverage] = new BeverageFactory(),
        };

        public DishFactory GetFactory(DishCategory category)
        {
            if (!factories.TryGetValue(category, out var factory))
                throw new NotSupportedException($"Category not supported: {category}");

            return factory;
        }
    }
}
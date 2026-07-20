using RestaurantApi.Dtos;
using RestaurantApi.Enums;
using RestaurantApi.Factories;
using RestaurantApi.Interfaces;
using RestaurantApi.Models;

namespace RestaurantApi.Singletons
{
    public sealed class RestaurantConfiguration
    {
        private static readonly Lazy<RestaurantConfiguration> instance = new(() =>
            new RestaurantConfiguration()
        );

        public static RestaurantConfiguration Instance => instance.Value;
        public string RestaurantName { get; }
        public string Currency { get; }
        public int TableCount { get; }
        public double ServiceChargeRate { get; }

        public IReadOnlyList<IDish> Menu { get; }
        public IReadOnlyList<OpeningHours> OpeningHours { get; }

        private RestaurantConfiguration()
        {
            RestaurantName = "Le Bistrot des Patterns";
            Currency = "EUR";
            TableCount = 20;
            ServiceChargeRate = 0.10;

            OpeningHours =
            [
                new(DayOfWeek.Monday, null, null),
                new(DayOfWeek.Tuesday, new(11, 30), new(22, 0)),
                new(DayOfWeek.Wednesday, new(11, 30), new(22, 0)),
                new(DayOfWeek.Thursday, new(11, 30), new(22, 0)),
                new(DayOfWeek.Friday, new(11, 30), new(23, 30)),
                new(DayOfWeek.Saturday, new(11, 30), new(23, 30)),
                new(DayOfWeek.Sunday, new(12, 0), new(15, 0)),
            ];

            Menu = BuildMenu();

            Console.WriteLine(
                $"[Singleton] RestaurantConfiguration initialized ({Menu.Count} dishes)."
            );
        }

        /// <summary>
        /// The menu is built using the existing factories: no "new Starter(...)" here.
        /// </summary>
        private static List<IDish> BuildMenu()
        {
            var provider = new DishFactoryProvider();

            CreateDishRequest[] catalogue =
            [
                new("Salade cesar", 8.5, DishCategory.Starter),
                new("Soupe a l'oignon", 7.0, DishCategory.Starter),
                new("Entrecote grillee", 22.0, DishCategory.MainCourse),
                new("Risotto aux champignons", 17.5, DishCategory.MainCourse),
                new("Tarte tatin", 6.5, DishCategory.Dessert),
                new("Mousse au chocolat", 5.5, DishCategory.Dessert),
                new("Verre de vin rouge", 5.0, DishCategory.Beverage, IsAlcoholic: true),
                new("Eau petillante", 3.0, DishCategory.Beverage, IsAlcoholic: false),
            ];

            return catalogue
                .Select(request => provider.GetFactory(request.Category).CreateDish(request))
                .ToList();
        }
    }
}
using RestaurantApi.Dtos;
using RestaurantApi.Interfaces;

namespace RestaurantApi.Factories
{
    public abstract class DishFactory
    {
        /// <summary>
        /// Temps de préparation utilisé quand la requête n'en précise aucun.
        /// Chaque catégorie définit le sien.
        /// </summary>
        protected abstract TimeSpan DefaultPreparationTime { get; }

        public abstract IDish CreateDish(CreateDishRequest request);

        protected TimeSpan ResolvePreparationTime(CreateDishRequest request)
        {
            return request.PreparationTimeMinutes.HasValue
                ? TimeSpan.FromMinutes(request.PreparationTimeMinutes.Value)
                : DefaultPreparationTime;
        }
    }
}

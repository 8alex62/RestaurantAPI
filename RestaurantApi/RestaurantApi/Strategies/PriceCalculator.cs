using RestaurantApi.Enums;
using RestaurantApi.Interfaces;
using RestaurantApi.Models;

namespace RestaurantApi.Strategies
{
    public class PriceCalculator
    {
        private readonly Dictionary<PricingPolicy, IPriceOrder> strategies = new()
        {
            [PricingPolicy.Standard] = new StandardStrategy(),
            [PricingPolicy.HappyHour] = new HappyHourStrategy(),
            [PricingPolicy.Group] = new GroupStrategy(),
            [PricingPolicy.Menu] = new MenuStrategy(),
        };

        public IPriceOrder GetStrategy(PricingPolicy policy)
        {
            if (!strategies.TryGetValue(policy, out var strategy))
                throw new NotSupportedException($"Politique non supportée : {policy}");

            return strategy;
        }

        public double CalculatePrice(Order order) =>
            GetStrategy(order.Policy).CalculatePrice(order);

        public string GetDescription(Order order) => GetStrategy(order.Policy).GetDescription();
    }
}
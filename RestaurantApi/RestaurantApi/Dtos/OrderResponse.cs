using RestaurantApi.Enums;
using RestaurantApi.Models;

namespace RestaurantApi.Dtos
{
    /// <summary>
    /// Vue JSON d'une commande : evite d'exposer l'objet OrderState (qui reference
    /// la commande) et expose simplement le nom de l'etat courant.
    /// </summary>
    public record OrderResponse(
        string Id,
        int TableNumber,
        List<DishResponse> Items,
        double TotalPrice,
        string Status,
        PricingPolicy Policy,
        string PricingDescription,
        DateTime CreatedAt
    )
    {
        public static OrderResponse From(Order order, string pricingDescription) =>
            new(
                order.Id,
                order.TableNumber,
                order.Items.Select(DishResponse.From).ToList(),
                order.TotalPrice,
                order.Status.GetStateName(),
                order.Policy,
                pricingDescription,
                order.CreatedAt
            );
    }
}

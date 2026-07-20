using RestaurantApi.Models;

namespace RestaurantApi.States
{
    public class PaidState(Order order) : OrderState(order)
    {
        protected override OrderState BuildNextState()
        {
            // Etat terminal : aucune transition possible, donc aucune notification.
            throw new InvalidOperationException("La commande est deja payee.");
        }
    }
}

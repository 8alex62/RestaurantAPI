using RestaurantApi.Models;

namespace RestaurantApi.States
{
    public class PreparationState(Order order) : OrderState(order)
    {
        protected override OrderState BuildNextState()
        {
            return new ReadyState(order);
        }
    }
}

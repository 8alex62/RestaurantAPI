using RestaurantApi.Models;

namespace RestaurantApi.States
{
    public class ReceivedState(Order order) : OrderState(order)
    {
        protected override OrderState BuildNextState()
        {
            return new PreparationState(order);
        }
    }
}

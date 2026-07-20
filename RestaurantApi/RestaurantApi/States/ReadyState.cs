using RestaurantApi.Models;

namespace RestaurantApi.States
{
    public class ReadyState(Order order) : OrderState(order)
    {
        protected override OrderState BuildNextState()
        {
            return new ServedState(order);
        }
    }
}

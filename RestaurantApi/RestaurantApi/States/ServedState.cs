using RestaurantApi.Models;

namespace RestaurantApi.States
{
    public class ServedState(Order order) : OrderState(order)
    {
        protected override OrderState BuildNextState()
        {
            return new PaidState(order);
        }
    }
}

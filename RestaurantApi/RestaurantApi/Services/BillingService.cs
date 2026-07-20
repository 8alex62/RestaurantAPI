using RestaurantApi.Interfaces;
using RestaurantApi.Models;
using RestaurantApi.States;

namespace RestaurantApi.Services
{
    public class BillingService : INotificationObserver
    {
        public string GetObserverName() => "Billing";

        public void Update(Order order)
        {
            switch (order.Status)
            {
                case ReceivedState:
                    Console.WriteLine(
                        $"  [Billing] Opened bill for table {order.TableNumber} : "
                            + $"{order.TotalPrice:0.00} EUR ({order.Policy})."
                    );
                    break;

                case PaidState:
                    Console.WriteLine(
                        $"  [Billing] Payment received: {order.TotalPrice:0.00} EUR "
                            + $"for table {order.TableNumber}. Bill closed."
                    );
                    break;
            }
        }
    }
}
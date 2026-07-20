using RestaurantApi.Interfaces;
using RestaurantApi.Models;
using RestaurantApi.States;

namespace RestaurantApi.Services
{
    public class DiningService : INotificationObserver
    {
        public string GetObserverName() => "Dining";

        public void Update(Order order)
        {
            if (order.Status is ReadyState)
            {
                Console.WriteLine(
                    $"  [Dining] Order ready: serve table {order.TableNumber} "
                        + $"({order.Items.Count} dish(es))."
                );
            }
        }
    }
}
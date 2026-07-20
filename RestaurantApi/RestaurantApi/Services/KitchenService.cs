using RestaurantApi.Interfaces;
using RestaurantApi.Models;
using RestaurantApi.States;

namespace RestaurantApi.Services
{
    public class KitchenService : INotificationObserver
    {
        public string GetObserverName() => "Kitchen";

        public void Update(Order order)
        {
            switch (order.Status)
            {
                case ReceivedState:
                    Console.WriteLine(
                        $"  [Kitchen] New order for table {order.TableNumber} : "
                            + $"{order.Items.Count} dish(es) to prepare."
                    );
                    foreach (var dish in order.Items)
                    {
                        Console.WriteLine(
                            $"  [Kitchen]   - {dish.Name} ({dish.PreparationTime.TotalMinutes} min)"
                        );
                    }
                    break;

                case PreparationState:
                    var total = order.Items.Sum(d => d.PreparationTime.TotalMinutes);
                    Console.WriteLine(
                        $"  [Kitchen] Start preparation for table {order.TableNumber} "
                            + $"(estimated time: {total} min)."
                    );
                    break;
            }
        }
    }
}
using RestaurantApi.Enums;
using RestaurantApi.Interfaces;
using RestaurantApi.States;

namespace RestaurantApi.Models
{
    public class Order : INotificationSubject
    {
        private readonly List<INotificationObserver> observers = new();

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int TableNumber { get; set; }
        public List<IDish> Items { get; set; } = new();
        public double TotalPrice { get; set; }
        public OrderState Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public PricingPolicy Policy { get; set; } = PricingPolicy.Standard;

        public Order()
        {
            this.Status = new ReceivedState(this);
        }

        public void AddItem(IDish item)
        {
            Items.Add(item);
        }

        public void Subscribe(INotificationObserver observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
        }

        public void Unsubscribe(INotificationObserver observer)
        {
            observers.Remove(observer);
        }

        public bool UnsubscribeByName(string observerName)
        {
            var observer = observers.FirstOrDefault(o =>
                o.GetObserverName().Equals(observerName, StringComparison.OrdinalIgnoreCase)
            );

            if (observer is null)
                return false;

            observers.Remove(observer);
            return true;
        }

        public IEnumerable<string> GetObserverNames()
        {
            return observers.Select(o => o.GetObserverName());
        }

        public void Notify(Order order)
        {
            Console.WriteLine(
                $"[Order {order.Id[..8]}] Status = {order.Status.GetStateName()} "
                    + $"-> Send to {observers.Count} service(s)"
            );

            foreach (var observer in observers.ToList())
            {
                observer.Update(order);
            }
        }
    }
}
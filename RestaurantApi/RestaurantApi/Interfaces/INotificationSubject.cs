using RestaurantApi.Models;

namespace RestaurantApi.Interfaces
{
    // Sujet observe : ne connait que l'interface INotificationObserver,
    // jamais les services concrets.
    public interface INotificationSubject
    {
        void Subscribe(INotificationObserver observer);

        void Unsubscribe(INotificationObserver observer);

        void Notify(Order order);
    }
}

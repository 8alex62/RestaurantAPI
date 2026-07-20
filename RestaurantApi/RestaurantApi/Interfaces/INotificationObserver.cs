using RestaurantApi.Models;

namespace RestaurantApi.Interfaces
{
    // Observateur : un service interesse par les evenements d'une commande.
    public interface INotificationObserver
    {
        void Update(Order order);

        string GetObserverName();
    }
}

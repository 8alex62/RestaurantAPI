using RestaurantApi.Models;

namespace RestaurantApi.States
{
    public abstract class OrderState(Order order)
    {
        protected Order order = order;

        // Chaque etat concret se contente de dire quel est l'etat suivant.
        protected abstract OrderState BuildNextState();

        // La transition et la diffusion de l'evenement sont centralisees ici :
        // c'est le seul point ou le State declenche l'Observer.
        public void NextState()
        {
            order.Status = BuildNextState();
            order.Notify(order);
        }

        public virtual string GetStateName()
        {
            return this.GetType().Name;
        }
    }
}

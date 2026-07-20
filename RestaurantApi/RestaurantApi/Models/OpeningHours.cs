namespace RestaurantApi.Models
{
    /// <summary>
    /// Creneau d'ouverture pour un jour de la semaine. Un jour de fermeture est
    /// represente par Opening et Closing nuls.
    /// </summary>
    public record OpeningHours(DayOfWeek Day, TimeOnly? Opening, TimeOnly? Closing)
    {
        public bool IsClosed => Opening is null || Closing is null;
    }
}

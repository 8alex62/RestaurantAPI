namespace RestaurantApi.Models
{
    public class Order
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int TableNumber { get; set; }
        public List<MenuItem> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Received";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public void AddItem(MenuItem item)
        {
            Items.Add(item);
        }
    }
}

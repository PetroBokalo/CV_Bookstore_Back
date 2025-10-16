namespace BookStoreAPI.Models
{
    public class Order
    {

        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public List<OrderItem> Items { get; set; } = new();

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }

    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; } = null!;

        public int BookId { get; set; }

        public Book Book { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal Price { get; set; } 
    }


    public enum OrderStatus
    {
        Pending,
        Paid,
        Shipped,
        Completted,
        Cancelled
    }

}
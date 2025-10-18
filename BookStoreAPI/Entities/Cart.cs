namespace BookStoreAPI.Entities
{
    public class Cart
    {

        public int Id { get; set; }

        public int AppUserId { get; set; }

        public AppUser AppUser { get; set; } = null!;

        public List<CartItem> Items { get; set; } = new();

    }

    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; }

        public Cart Cart { get; set; } = null!;


        public int BookId { get; set; }

        public Book Book { get; set; } = null!;

        public int Quantity { get; set; }   

    }
}



namespace BookStore.Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
        public bool HasDiscount { get; set; }

        public byte DiscountPercantage
        {
            get
            {
                if (!HasDiscount || Price == 0) return 0;
                return (byte)Math.Round((Price - DiscountPrice) / Price * 100);
            }
        }

        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public List<BookImage> Images { get; set; } = new();
        public string Publisher { get; set; } = string.Empty;
        public string? Translator { get; set; }
        public bool IsTranslated => !string.IsNullOrWhiteSpace(Translator);
        public int PublicationYear { get; set; }
        public int PagesNumber { get; set; }
        public BookCover Cover { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public BookFormat Format { get; set; } = new();
        public BookType BookType { get; set; }
        public bool HasIllustrations { get; set; }
        public int SoldAmount { get; set; }
        public DateTime AddedTime { get; set; } = DateTime.UtcNow;
        public int Capacity { get; set; }
        public int BookGenreId { get; set; }
        public BookGenre BookGenre { get; set; } = null!;
        public ICollection<BookGenre> Genres { get; set; } = new List<BookGenre>();
    }

    public enum BookType
    {
        PaperBook,
        ElectronicBook
    }

    public enum BookCover
    {
        Soft,
        Hard
    }

    public class BookImage
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public byte[] ImageData { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "image/jpeg";
    }

    public class BookFormat
    {
        public int WidthMm { get; set; }
        public int HeightMm { get; set; }
        public int ThicknessMm { get; set; }
    }
}

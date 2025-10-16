using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreAPI.Models
{
    public class Book
    {

        public int Id { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DiscountPrice { get; set; }

        public bool HasDiscount { get; set;}

        [Range(0, 100)]
        [NotMapped]
        public byte DiscountPercantage 
        {
            get
            {
                if (!HasDiscount) return 0;
                return (byte)Math.Round((Price - DiscountPrice) / Price * 100);
            }
        }    

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        [Required]
        public List<BookImage> Images { get; set; } = new();

        [Required]
        public string Publisher { get; set; } = string.Empty;
            
        public string? Translator { get; set; }
        public bool IsTranslated => !string.IsNullOrWhiteSpace(Translator);

        [Required]
        [Range(1450, 2100, ErrorMessage = "Рік має бути в межах 1450-2100")]
        public int PublicationYear { get; set; }

        [Required]
        public int PagesNumber { get; set; }

        [Required]
        public BookCover Cover { get; set; }

        [Required]
        [RegularExpression(@"^(97(8|9))?\d{9}(\d|X)$", ErrorMessage = "Невірний ISBN")]
        [MaxLength(13)]
        public string ISBN { get; set; } = string.Empty;

        [Required]
        public BookFormat Format { get; set; } = new();

        [Required]
        public BookType BookType { get; set; }

        [Required]
        public bool HasIllustrations { get; set; }

        public int SoldAmount { get; set; }

        public DateTime AddedTime { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(0,int.MaxValue)]
        public int Capacity { get; set; }

        public int BookGenreId { get; set; }

        public BookGenre BookGenre { get; set; } = null!;

    }

    public enum BookType
    {
        PaperBook, // паперова книга
        ElectronicBook // електронна книга
    }

    public enum BookCover
    {
        Soft, // м'яка
        Hard // тверда
    }

    public class BookImage
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        public byte[] ImageData { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "image/jpeg"; 

    }

    [Owned]
    public class BookFormat
    {
        [Required]
        public int WidthMm { get; set; }
        [Required]
        public int HeightMm { get; set; }
        public int ThicknessMm { get; set; }
    }

}

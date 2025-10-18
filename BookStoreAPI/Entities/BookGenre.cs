using System.ComponentModel.DataAnnotations;

namespace BookStoreAPI.Entities
{
    public class BookGenre
    {

        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }


        public ICollection<Book> Books { get; set; } = new List<Book>(); // для зв'язку багато до багатьох

    }
}

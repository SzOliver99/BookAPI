using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace bookapi.Entities
{
    public class Author
    {
        public int Id { get; set; }
        [StringLength(255)]
        public string? Name { get; set; }
        public DateTime BirthDate { get; set; }
        
        public string? UserId { get; set; }
        public User? User { get; set; }
        public List<Book> Books { get; set; } = new();
    }
}

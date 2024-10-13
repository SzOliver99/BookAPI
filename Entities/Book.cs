using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace bookapi.Entities
{
    public class Book
    {
        public int Id { get; set; }
        [StringLength(255)]
        public string? Title { get; set; }
        public DateTime PublishedDate { get; set; }
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
    }
}

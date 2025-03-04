using System.ComponentModel.DataAnnotations;

namespace AkidoTrainingWebAPI.DataAccess.Models
{
    public class Posts
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Writer { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

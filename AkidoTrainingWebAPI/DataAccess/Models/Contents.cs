using System.ComponentModel.DataAnnotations;

namespace AkidoTrainingWebAPI.DataAccess.Models
{
    public class Contents
    {
        [Key]
        public int Id { get; set; }
        public int PostId { get; set; }
        public string? Content { get; set; }
        public int Order {  get; set; }
        public string? Type { get; set; }
    }
}

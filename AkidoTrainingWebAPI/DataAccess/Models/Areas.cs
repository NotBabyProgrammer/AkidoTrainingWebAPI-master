using System.ComponentModel.DataAnnotations;

namespace AkidoTrainingWebAPI.DataAccess.Models
{
    public class Areas
    {
        [Key]
        public int Id { get; set; }
        public string? District { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public List<DateTime>? Schedule { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
    }
}

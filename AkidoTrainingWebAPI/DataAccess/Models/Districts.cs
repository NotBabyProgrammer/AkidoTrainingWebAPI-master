using System.ComponentModel.DataAnnotations;

namespace AkidoTrainingWebAPI.DataAccess.Models
{
    public class Districts
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace AkidoTrainingWebAPI.DataAccess.Models
{
    public class Roles
    {
        [Key]
        public int RolesId { get; set; }
        [Required]
        public string? RoleName { get; set; }
        public string? RoleDescription { get; set;}
    }
}

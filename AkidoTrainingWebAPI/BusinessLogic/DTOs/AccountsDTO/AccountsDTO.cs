using System.ComponentModel.DataAnnotations;

namespace AkidoTrainingWebAPI.BusinessLogic.DTOs.AccountsDTO
{
    public class AccountsDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? Belt { get; set; }
        public string? ImagePath { get; set; }
        public int? Level { get; set; }
    }
}

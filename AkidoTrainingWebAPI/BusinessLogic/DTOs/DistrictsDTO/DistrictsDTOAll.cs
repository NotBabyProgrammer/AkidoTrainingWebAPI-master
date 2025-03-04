using System.ComponentModel.DataAnnotations;

namespace AkidoTrainingWebAPI.BusinessLogic.DTOs.DistrictsDTO
{
    public class DistrictsDTOAll
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}

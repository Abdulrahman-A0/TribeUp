using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.ProfileModule
{
    public record UpdateBioDTO
    {
        [MaxLength(200)]
        public string Bio { get; init; }
    }
}

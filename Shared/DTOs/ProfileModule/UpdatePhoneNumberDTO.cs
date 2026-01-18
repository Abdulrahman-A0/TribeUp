using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.ProfileModule
{
    public record UpdatePhoneNumberDTO
    {
        [Phone]
        public string PhoneNumber { get; init; } = null!;
    }

}

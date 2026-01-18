using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.ProfileModule
{
    public record UpdateProfileDTO
    {
        [Required(ErrorMessage = "First name can't be empty")]
        public string FirstName { get; init; }

        [Required(ErrorMessage = "Last name can't be empty")]
        public string LastName { get; init; }
    }

}

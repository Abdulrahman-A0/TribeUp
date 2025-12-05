using Shared.DTOs.IdentityModule;

namespace ServiceAbstraction.Contracts
{
    public interface IAuthenticationService
    {
        Task<UserResultDTO> RegisterAsync(RegisterDTO registerDTO);
        Task<UserResultDTO> LoginAsync(LoginDTO loginDTO);
    }
}

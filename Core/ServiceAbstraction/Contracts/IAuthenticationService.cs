using Shared.DTOs.IdentityModule;

namespace ServiceAbstraction.Contracts
{
    public interface IAuthenticationService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDTO, string deviceId);
        Task<AuthResponseDTO> LoginAsync(LoginDTO loginDTO, string deviceId);
        Task<AuthResponseDTO> RefreshAsync(RefreshTokenRequestDTO refreshTokenDTO);
        Task LogoutAsync(string userId, string deviceId);
    }
}

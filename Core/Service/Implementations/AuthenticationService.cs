using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Users;
using Domain.Exceptions.UnAuthorized;
using Domain.Exceptions.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Specifications;
using Service.Specifications.RefreshTokenSpecifications;
using ServiceAbstraction.Contracts;
using Shared.Common;
using Shared.DTOs.IdentityModule;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Service.Implementations
{
    public class AuthenticationService(UserManager<ApplicationUser> _userManager,
        IOptions<JwtOptions> _options, IMapper _mapper, IUnitOfWork unitOfWork,
        IConfiguration configuration, IEmailService emailService) : IAuthenticationService
    {
        public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDTO, string deviceId)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user is null) throw new UnAuthorizedException();

            var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!result) throw new UnAuthorizedException();

            foreach (var rt in user.RefreshTokens.Where(rt => rt.DeviceId == deviceId))
                rt.IsRevoked = true;

            var token = await CreateTokenAsync(user);

            var refreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(new RefreshToken
            {
                TokenHash = HashToken(refreshToken),
                DeviceId = deviceId,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                IsRevoked = false
            });

            await _userManager.UpdateAsync(user);


            return new AuthResponseDTO(token, refreshToken);
        }

        public async Task<AuthResponseDTO> RefreshAsync(RefreshTokenRequestDTO refreshTokenDTO)
        {
            var tokenHash = HashToken(refreshTokenDTO.RefreshToken);

            var specification = new RefreshTokenByHashAndDeviceSpec(tokenHash, refreshTokenDTO.DeviceId);

            var storedToken = await unitOfWork
                .GetRepository<RefreshToken, Guid>()
                .GetByIdAsync(specification);

            if (storedToken is null)
                throw new UnAuthorizedException("Invalid refresh token");

            if (storedToken.IsRevoked)
                throw new UnAuthorizedException("Refresh token revoked");

            if (storedToken.ExpiresAt < DateTime.UtcNow)
                throw new UnAuthorizedException("Refresh token expired");

            storedToken.IsRevoked = true;

            var newRefreshToken = GenerateRefreshToken();

            storedToken.User.RefreshTokens.Add(new RefreshToken
            {
                TokenHash = HashToken(newRefreshToken),
                DeviceId = refreshTokenDTO.DeviceId,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                IsRevoked = false
            });

            await unitOfWork.SaveChangesAsync();

            var newAccessToken = await CreateTokenAsync(storedToken.User);

            return new AuthResponseDTO(newAccessToken, newRefreshToken);
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDTO, string deviceId)
        {
            var user = _mapper.Map<ApplicationUser>(registerDTO);

            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                throw new ValidationException(errors);
            }

            var token = await CreateTokenAsync(user);

            var refreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(new RefreshToken
            {
                TokenHash = HashToken(refreshToken),
                DeviceId = deviceId,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                IsRevoked = false
            });

            await _userManager.UpdateAsync(user);


            return new AuthResponseDTO(token, refreshToken);
        }

        public async Task LogoutAsync(string userId, string deviceId)
        {
            var specification = new ActiveRefreshTokensByUserAndDeviceSpec(userId, deviceId);

            var tokens = await unitOfWork
                .GetRepository<RefreshToken, Guid>()
                .GetAllAsync(specification);

            foreach (var token in tokens)
                token.IsRevoked = true;

            await unitOfWork.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(string userId, ChangePasswordDTO changePasswordDTO)
        {
            if (changePasswordDTO.NewPassword != changePasswordDTO.ConfirmNewPassword)
                throw new ValidationException(["Passwords do not match"]);

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new UnAuthorizedException();

            var result = await _userManager.ChangePasswordAsync(
                user,
                changePasswordDTO.CurrentPassword,
                changePasswordDTO.NewPassword);

            if (!result.Succeeded)
                throw new ValidationException(result.Errors.Select(e => e.Description).ToList());


            foreach (var token in user.RefreshTokens)
                token.IsRevoked = true;

            await _userManager.UpdateAsync(user);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDTO.Email);
            if (user is null)
                return;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink =
                $"{configuration.GetSection("URLs:FrontUrl")}/reset-password" +
                $"?email={Uri.EscapeDataString(user.Email!)}" +
                $"&token={Uri.EscapeDataString(token)}";

            await emailService.SendPasswordResetAsync(user.Email!, resetLink);
        }

        public async Task ResetPasswordAsync(ResetPasswordDTO dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                throw new ValidationException(["Passwords do not match"]);

            var user = await _userManager.FindByEmailAsync(dto.Email)
                ?? throw new UnAuthorizedException();

            var decodedToken = Uri.UnescapeDataString(dto.Token);

            var result = await _userManager.ResetPasswordAsync(
                user,
                decodedToken,
                dto.NewPassword);

            if (!result.Succeeded)
                throw new ValidationException(result.Errors.Select(e => e.Description).ToList());

            foreach (var token in user.RefreshTokens)
                token.IsRevoked = true;

            await _userManager.UpdateAsync(user);
        }



        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var JwtOptions = _options.Value;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email,user.Email),
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.SecretKey));

            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: JwtOptions.Issuer,
                audience: JwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(JwtOptions.ExpirationInMinutes),
                signingCredentials: signingCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            return Convert.ToBase64String(sha256.ComputeHash(bytes));
        }
    }
}

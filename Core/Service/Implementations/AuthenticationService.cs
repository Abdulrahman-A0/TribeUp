using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Users;
using Domain.Exceptions.AuthExceptions;
using Domain.Exceptions.UserExceptions;
using Domain.Exceptions.ValidationExceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
            if (user is null) throw new AuthenticationFailedException("invalid_credentials");

            var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!result) throw new AuthenticationFailedException("invalid_credentials");

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

            if (storedToken is null ||
                storedToken.IsRevoked ||
                storedToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new AuthenticationFailedException("invalid_refresh_token");
            }

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
                var errors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToArray());
                throw new DomainValidationException(errors);
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
                throw new DomainValidationException(
                    new Dictionary<string, string[]>
                    {
                        ["ConfirmNewPassword"] = new[] { "Passwords do not match" }
                    });

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new UserNotFoundException(userId);

            var result = await _userManager.ChangePasswordAsync(
                user,
                changePasswordDTO.CurrentPassword,
                changePasswordDTO.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToArray());
                throw new DomainValidationException(errors);
            }


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
                $"{configuration["URLs:FrontUrl"]}/reset-password" +
                $"?email={Uri.EscapeDataString(user.Email!)}" +
                $"&token={Uri.EscapeDataString(token)}";

            await emailService.SendPasswordResetAsync(user.Email!, resetLink);
        }

        public async Task ResetPasswordAsync(ResetPasswordDTO dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                throw new DomainValidationException(
                    new Dictionary<string, string[]>
                    {
                        ["ConfirmNewPassword"] = new[] { "Passwords do not match" }
                    });

            var user = await _userManager.FindByEmailAsync(dto.Email)
                ?? throw new AuthenticationFailedException("invalid_reset_request");

            var decodedToken = Uri.UnescapeDataString(dto.Token);

            var result = await _userManager.ResetPasswordAsync(
                user,
                decodedToken,
                dto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToArray());
                throw new DomainValidationException(errors);
            }

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
                new Claim(ClaimTypes.Name,user.UserName)
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

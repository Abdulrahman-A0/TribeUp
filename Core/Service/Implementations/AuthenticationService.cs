using AutoMapper;
using Domain.Entities.Users;
using Domain.Exceptions.UnAuthorized;
using Domain.Exceptions.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServiceAbstraction.Contracts;
using Shared.Common;
using Shared.DTOs.IdentityModule;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Service.Implementations
{
    public class AuthenticationService(UserManager<ApplicationUser> _userManager,
        IOptions<JwtOptions> _options, IMapper _mapper) : IAuthenticationService
    {
        public async Task<UserResultDTO> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user is null) throw new UnAuthorizedException();

            var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!result) throw new UnAuthorizedException();

            var token = await CreateTokenAsync(user);

            return _mapper.Map<UserResultDTO>(user) with { Token = token };
        }

        public async Task<UserResultDTO> RegisterAsync(RegisterDTO registerDTO)
        {
            var user = _mapper.Map<ApplicationUser>(registerDTO);

            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                throw new ValidationException(errors);
            }

            var token = await CreateTokenAsync(user);

            return _mapper.Map<UserResultDTO>(user) with { Token = token };
        }

        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var JwtOptions = _options.Value;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
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
                expires: DateTime.UtcNow.AddDays(JwtOptions.ExpirationInDays),
                signingCredentials: signingCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

using HabitTracker.Server.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HabitTracker.Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            IdentityUser user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return null;
            }

            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (isPasswordValid)
            {
                return new AuthResponseDto { Token = GenerateJwtToken(user), Email = dto.Email, Expiration = GetTokenExpiration() };

            }
            else
            {
                return null;
            }
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = dto.Email,
                Email = dto.Email
            };

            IdentityResult userResult = await _userManager.CreateAsync(user, dto.Password);
            if (userResult.Succeeded)
            {
                return new AuthResponseDto { Token = GenerateJwtToken(user), Email = dto.Email, Expiration = GetTokenExpiration() };
            }

            return null;
        }

        public string GenerateJwtToken(IdentityUser user)
        {
            Claim[] claims =
             [
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            ];

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: GetTokenExpiration(),
                signingCredentials: creds
            );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        private DateTime GetTokenExpiration()
        {
            var expirationHours = int.Parse(_configuration["Jwt:ExpirationHours"]);
            return DateTime.UtcNow.AddHours(expirationHours);
        }
    }
}

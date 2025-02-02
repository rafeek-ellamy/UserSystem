using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserSystem.Data.Entities;

namespace UserSystem.Services.UtilityHelper
{
    public class UtilityHelper : IUtilityHelper
    {
        #region Ctor
        private readonly UserManager<UserProfile> _userManager;
        private readonly IConfiguration _configuration;

        public UtilityHelper(UserManager<UserProfile> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        #endregion


        public async Task<string> GenerateSecurityToken(UserProfile user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("UserId", !string.IsNullOrEmpty(user.Id) ? user.Id : string.Empty),
                new Claim("FirstName", !string.IsNullOrEmpty(user.FirstName) ? user.FirstName : string.Empty),
                new Claim("LastName", !string.IsNullOrEmpty(user.LastName) ? user.LastName : string.Empty),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim("roles", role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:Key"] ?? throw new InvalidOperationException()));

            var creditials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Token:Issuer"],
                _configuration["Token:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("AccessTokenExpireTimeByMinutes")),
                signingCredentials: creditials,
                notBefore: DateTime.UtcNow
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

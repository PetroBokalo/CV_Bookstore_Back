using BookStoreAPI.Common;
using BookStoreAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStoreAPI.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        public TokenService(IConfiguration config)
        {
            _config = config;

            _secretKey = config["Jwt:Key"]!;
            _issuer = config["Jwt:Issuer"]!;
            _audience = config["Jwt:Audience"]!;

        }

        public string GenerateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("role", "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            
               issuer: _issuer,
               audience: _audience,
               claims: claims,
               expires: DateTime.UtcNow.AddMinutes(15),
               signingCredentials: creds); 


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        public ServiceResult<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
                ValidateLifetime = false,
                ValidIssuer = _issuer,
                ValidAudience = _audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return ServiceResult<ClaimsPrincipal>.Fail("Invalid Token");
            }

            return ServiceResult<ClaimsPrincipal>.Ok(principal, "Token is refreshed");
        }

    }
}

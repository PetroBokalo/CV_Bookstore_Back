using BookStoreAPI.Common;
using BookStoreAPI.Entities;
using BookStoreAPI.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BookStoreAPI.Services.Implementations
{
    public class TokenService : ITokenService
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

        public string GenerateAccessToken(AppUser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("role", "AppUser")
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

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        public string GenerateVerifyToken()
        {
            var bytes = new byte[4];
            RandomNumberGenerator.Fill(bytes);
            int value = BitConverter.ToInt32(bytes, 0);
            value = Math.Abs(value % 1000000);
            var code = value.ToString("D6");
            return code;
        }


    }
}

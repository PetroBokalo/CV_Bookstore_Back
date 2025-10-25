using BookStoreAPI.Entities;

namespace BookStoreAPI.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(AppUser user);
        string GenerateRefreshToken();
        string GenerateVerifyToken();
    }
}
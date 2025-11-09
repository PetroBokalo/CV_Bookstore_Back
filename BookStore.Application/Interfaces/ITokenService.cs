
namespace BookStore.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(int userId, string userEmail, bool emailVerified);
        string GenerateRefreshToken();
        string GenerateVerifyToken();
    }
}
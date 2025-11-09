namespace BookStore.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendResetPasswordLinkAsync(string toEmail, string link, DateTime expiry);
        Task SendVerificationCodeAsync(string toEmail, string code, DateTime expiry);
    }
}
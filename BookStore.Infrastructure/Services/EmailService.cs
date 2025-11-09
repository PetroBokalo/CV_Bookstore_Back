using BookStore.Application.Common;
using BookStore.Application.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace BookStoreAPI.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendVerificationCodeAsync(string toEmail, string code, DateTime expiry)
        {
            var local_expiry = expiry.ToLocalTime();

            var from = new MailAddress(_settings.FromEmail, _settings.FromName);
            var to = new MailAddress(toEmail);
            var subject = "Verify your email";
            var body = $@"
                <html>
                  <body style='font-family: Arial, sans-serif; color: #202124; line-height:1.6;'>
                    <h2 style='color:#202124; margin-bottom:10px;'>Email Verification</h2>
                    <p style='color:#202124;'>Your verification code is:</p>
                    <h3 style='color:#1a73e8; font-size:24px; letter-spacing:2px;'>{code}</h3>
                    <p style='color:#202124;'>This code will expire on <b>{local_expiry:yyyy-MM-dd HH:mm:ss}</b>.</p>
                    <p style='color:#202124;'>If you didn’t request this code, please ignore this message.</p>
                  </body>
                </html>";


            using var smtp = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.SmtpUser, _settings.SmtpPass),
                EnableSsl = true
            };

            using var message = new MailMessage(from, to)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(message);
        }


        public async Task SendResetPasswordLinkAsync(string toEmail, string link, DateTime expiry)
        {
            var local_expiry = expiry.ToLocalTime();
            var from = new MailAddress(_settings.FromEmail, _settings.FromName);
            var to = new MailAddress(toEmail);
            var subject = "Password recovery";
            var body = $@"
                <html>
                  <body style='font-family: Arial, sans-serif; color: #202124; line-height:1.6;'>
                    <h3 style='color:#202124; font-size:20px; letter-spacing:1px;'>Rassword reset</h3>
                    <p style='color:#202124;'>Click the link below to reset your password</p>
                    <p>
                        <a href='{link}'
                            style='display:inline-block;
                                background-color:#1a73e8;
                                color:white;
                                padding:10px 18px;
                                text-decoration:none;
                                border-radius:6px;
                                font-weight:bold;'>
                        Reset Password
                      </a>
                    </p>
                    <p style='color:#202124;'>This link will expire on <b>{local_expiry:yyyy-MM-dd HH:mm:ss}</b>.</p>
                    <p style='color:#202124;'>If you didn’t request this link, please ignore this message.</p>
                  </body>
                </html>";

            using var smtp = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.SmtpUser, _settings.SmtpPass),
                EnableSsl = true
            };

            using var message = new MailMessage(from, to)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            await smtp.SendMailAsync(message);
        }


    }
}

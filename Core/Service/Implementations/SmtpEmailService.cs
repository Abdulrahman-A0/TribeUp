using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using ServiceAbstraction.Contracts;
using Shared.Common;
using System.Net;
using System.Net.Mail;

namespace Service.Implementations
{
    public class SmtpEmailService(IOptions<EmailOptions> options, IWebHostEnvironment environment) : IEmailService
    {
        public async Task SendPasswordResetAsync(string email, string resetLink)
        {
            var emailOptions = options.Value;

            var templatePath = Path.Combine(
                environment.ContentRootPath,
                "Email",
                "Templates",
                "ResetPassword.html");


            var html = await File.ReadAllTextAsync(templatePath);

            html = html.Replace("{{RESET_LINK}}", resetLink);

            var message = new MailMessage
            {
                From = new MailAddress(emailOptions.From),
                Subject = "Reset your password",
                Body = html,
                IsBodyHtml = true
            };

            message.To.Add(email);

            using var smtp = new SmtpClient(emailOptions.Host, emailOptions.Port)
            {
                Credentials = new NetworkCredential(
                    emailOptions.Username,
                    emailOptions.Password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(message);
        }
    }
}

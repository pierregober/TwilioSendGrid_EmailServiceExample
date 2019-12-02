using Example.Data.Providers;
using Example.Models.Requests;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Example.Services
{
    public class EmailService : IEmailService
    {
        IDataProvider _data = null;
        public EmailService(IDataProvider data)
        {
            _data = data;
        }
        private async Task<Response> Send(SendGridMessage message)
        {
            var apiKey = Environment.GetEnvironmentVariable("TESTER");
            var client = new SendGridClient(apiKey);
            return await client.SendEmailAsync(message);
        }
        public async Task<Response> ResetEmail(string email, Guid token)
        {
            SendGridMessage message = new SendGridMessage();
            string directory = Environment.CurrentDirectory;
            string path = Path.Combine(directory, "EmailTemplates\\ResetPassword.html");
            string _htmlContent = System.IO.File.ReadAllText(path);
            string htmlContent = _htmlContent.Replace("{{resetLink}}", "https://localhost:3000/resetpassword/" + token);
            string plainTextContent = "Reset Password";

            var from = new EmailAddress("admin@example.org", "Pierre is Admin");
            var subject = "Please Reset Password";
            var to = new EmailAddress(email, "User");
            message = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            return await Send(message);
        }
    }
}

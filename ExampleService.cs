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
            return  await client.SendEmailAsync(message);
        }
        public async Task<Response> ConfirmEmail(string email, Guid token)
        {
            string confirmSubject = "Please confirm your account";  
            var model = new EmailAddRequest();
            model.To = email;
            model.From = "admin@example.org";
            model.Subject = confirmSubject;

            string directory = Environment.CurrentDirectory;
            string path = Path.Combine(directory, "EmailTemplates\\EmailConfirm.html");
            string _htmlContent = System.IO.File.ReadAllText(path);
            string content = _htmlContent.Replace("{{confirmLink}}", "https://localhost:3000/confirm/" + token);

            SendGridMessage message = new SendGridMessage()
            { 
                From = new EmailAddress(model.From), 
                Subject = model.Subject,
                HtmlContent = content
            };
            message.AddTo(model.To);
           return  await Send(message);
        }
    }
}

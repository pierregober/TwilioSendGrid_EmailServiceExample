namespace Sabio.Services
{
    public class EmailService : IEmailService
    {
        IDataProvider _data = null;
        public EmailService(IDataProvider data)
        {
            _data = data;
        }
        private async Task<Response> Send(SendGridMessage message, SendGridClientOptions apiKey)
        {
            var client = new SendGridClient(apiKey);
            return await client.SendEmailAsync(message);
        }
       
        public async Task<Response> SendMultiple(EmailListAddRequest model, SendGridClientOptions apiKey)
        {
            List<EmailAddress> list = null;
            string directory = Environment.CurrentDirectory;
            string path = Path.Combine(directory, "EmailTemplates\\ResponseEmail.html");
            string _htmlContent = System.IO.File.ReadAllText(path);
            string content = _htmlContent.Replace("{{confirmLink}}", "http://www.linkedin.com/in/pierregober");

            foreach (var em in model.Emails)
            {
                EmailAddress _email = new EmailAddress(em);
                if (list == null)
                {
                    list = new List<EmailAddress>();
                }
                list.Add(_email);
            }

            SendGridMessage message = new SendGridMessage()
            {
                From = new EmailAddress("admin@example.org"),
                Subject = "Update info",
                HtmlContent = content
            };
            message.AddTo("pierregober.com");
            message.AddBccs(list);

            return await Send(message, apiKey);
        }
		
        public async Task<Response> ResetEmail(string email, Guid token, SendGridClientOptions apiKey)
        {
            SendGridMessage message = new SendGridMessage();
            string directory = Environment.CurrentDirectory;
            string path = Path.Combine(directory, "EmailTemplates\\ResetPassword.html");
            string _htmlContent = System.IO.File.ReadAllText(path);
            string htmlContent = _htmlContent.Replace("{{resetLink}}", "https://www.exampleReplaceLink554.com" + token);
            string plainTextContent = "Reset Password";
            var from = new EmailAddress("admin@example.org", "Pierre is Admin");
            var subject = "Please Reset Password";
            var to = new EmailAddress(email, "User");
            message = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            return await Send(message, apiKey);
        }

        public async Task<Response> EmailResourcePdf(IFormFile file, string email, SendGridClientOptions apiKey)
        {
            string directory = Environment.CurrentDirectory;
            string path = Path.Combine(directory, "EmailTemplates\\EmailResourcesPdf.html");
            string _htmlContent = System.IO.File.ReadAllText(path);
            SendGridMessage message = new SendGridMessage()
            {
                From = new EmailAddress("admin@example.org"),
                Subject = "Your attached PDF!",
                HtmlContent = _htmlContent
            };          
           
            var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);
            var fileBytes = memoryStream.ToArray();
            string base64String = Convert.ToBase64String(fileBytes);            
            message.AddTo(email);
            message.AddAttachment("AttachmentName.pdf", base64String, "application/pdf");            

            return await Send(message, apiKey);
        }
    }
}

    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using MimeKit;
    using MailKit.Net.Smtp;
    using MailKit.Security;

    namespace Service_Academy1.Services
    {
        public class EmailService
        {
            private readonly IConfiguration _configuration;

            public EmailService(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public async Task SendEmailAsync(string toEmail, string subject, string body, string replyToEmail)
            {
                {
                    var smtpSettings = _configuration.GetSection("EmailSettings");

                    // Create the email message
                    var emailMessage = new MimeMessage();
                    emailMessage.From.Add(new MailboxAddress(smtpSettings["SenderName"], smtpSettings["SenderEmail"]));
                    emailMessage.To.Add(new MailboxAddress(string.Empty, toEmail)); // Here, empty for display name

                    // Ensure that replyToEmail is not null or empty
                    if (!string.IsNullOrEmpty(replyToEmail))
                    {
                        emailMessage.ReplyTo.Add(new MailboxAddress(string.Empty, replyToEmail)); // Set Reply-To with email address
                    }

                    emailMessage.Subject = subject;

                    // Create the body of the email
                    var bodyBuilder = new BodyBuilder
                    {
                        HtmlBody = body // You can use plain text as well if needed
                    };
                    emailMessage.Body = bodyBuilder.ToMessageBody();

                    // Configure the SMTP client
                    using (var smtpClient = new SmtpClient())
                    {
                        await smtpClient.ConnectAsync(smtpSettings["SMTPServer"], int.Parse(smtpSettings["Port"]), SecureSocketOptions.StartTls);
                        await smtpClient.AuthenticateAsync(smtpSettings["Username"], smtpSettings["Password"]);
                        await smtpClient.SendAsync(emailMessage);
                        await smtpClient.DisconnectAsync(true);
                    }
                }
            }
            public async Task SendSystemEmailAsync(string toEmail, string subject, string body, string replyToEmail = null)
            {
                var smtpSettings = _configuration.GetSection("EmailSettings");

                // Create the email message
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(smtpSettings["SenderName"], smtpSettings["SenderEmail"]));
                emailMessage.To.Add(new MailboxAddress(string.Empty, toEmail)); // Here, empty for display name

                // Ensure that replyToEmail is not null or empty
                if (!string.IsNullOrEmpty(replyToEmail))
                {
                    emailMessage.ReplyTo.Add(new MailboxAddress(string.Empty, replyToEmail)); // Set Reply-To with email address
                }

                emailMessage.Subject = subject;

                // Create the body of the email
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body // You can use plain text as well if needed
                };
                emailMessage.Body = bodyBuilder.ToMessageBody();

                // Configure the SMTP client
                using (var smtpClient = new SmtpClient())
                {
                    await smtpClient.ConnectAsync(smtpSettings["SMTPServer"], int.Parse(smtpSettings["Port"]), SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(smtpSettings["Username"], smtpSettings["Password"]);
                    await smtpClient.SendAsync(emailMessage);
                    await smtpClient.DisconnectAsync(true);
                }
            }
        public async Task SendEmailWithAttachmentAsync(string toEmail, string subject, string body, string replyToEmail, byte[] attachmentContent, string attachmentFileName)
        {
            var smtpSettings = _configuration.GetSection("EmailSettings");

            // Create the email message
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(smtpSettings["SenderName"], smtpSettings["SenderEmail"]));
            emailMessage.To.Add(new MailboxAddress(string.Empty, toEmail)); // Here, empty for display name

            if (!string.IsNullOrEmpty(replyToEmail))
            {
                emailMessage.ReplyTo.Add(new MailboxAddress(string.Empty, replyToEmail));
            }

            emailMessage.Subject = subject;

            // Create the body of the email
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body // You can use plain text as well if needed
            };

            // Attach the generated certificate PDF
            bodyBuilder.Attachments.Add(attachmentFileName, attachmentContent, ContentType.Parse("application/pdf"));
            emailMessage.Body = bodyBuilder.ToMessageBody();

            // Configure the SMTP client
            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync(smtpSettings["SMTPServer"], int.Parse(smtpSettings["Port"]), SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(smtpSettings["Username"], smtpSettings["Password"]);
                await smtpClient.SendAsync(emailMessage);
                await smtpClient.DisconnectAsync(true);
            }
        }

    }
}

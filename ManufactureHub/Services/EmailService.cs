using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace ESchedule.Services
{
    public class EmailService : IEmailSender
    {
        private readonly ILogger _logger;

        public EmailService(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<EmailService> logger)
        {
            Options = optionsAccessor.Value;
            _logger = logger;
        }

        public AuthMessageSenderOptions Options { get; } //Set with Secret Manager.

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrEmpty(Options.SendGridKey))
            {
                throw new Exception("Null SendGridKey");
            }
            await Execute(Options.SendGridKey, subject, htmlMessage, email);
        }

        public async Task Execute(string apiKey, string subject, string message, string toEmail)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("admin@manufactureHub.com", "Підтвердження пошти або скидання паролю"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(toEmail));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);
            var response = await client.SendEmailAsync(msg);
            _logger.LogInformation(response.IsSuccessStatusCode
                                   ? $"Email to {toEmail} queued successfully!"
                                   : $"Failure Email to {toEmail}");
        }

        //public async Task SendEmailAsync(string email,string fullname, string subject, string message)
        //{
        //    var emailMessage = new MimeMessage();

        //    emailMessage.From.Add(new MailboxAddress("Підтвердження пошти", "admin@eschedule.com"));
        //    emailMessage.To.Add(new MailboxAddress(fullname, email));
        //    emailMessage.Subject = subject;
        //    emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        //    {
        //        Text = message
        //    };

        //    using (var client = new SmtpClient())
        //    {
        //        await client.ConnectAsync("smtp.gmail.com", 587, false);
        //        await client.AuthenticateAsync("norkov900@gmail.com", "wttwcsefffytmkpz");
        //        await client.SendAsync(emailMessage);

        //        await client.DisconnectAsync(true);
        //    }
        //}
    }
}

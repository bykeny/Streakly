using Microsoft.AspNetCore.Identity.UI.Services;

namespace HabitGoalTrackerApp.Services.Implementation
{
    /// <summary>
    /// Development email sender that logs emails to the console instead of sending them.
    /// For production, replace this with a real email service (SendGrid, Mailgun, SMTP, etc.)
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // In development, log the email to the console
            _logger.LogInformation("=== EMAIL NOTIFICATION ===");
            _logger.LogInformation("To: {Email}", email);
            _logger.LogInformation("Subject: {Subject}", subject);
            _logger.LogInformation("Body: {HtmlMessage}", htmlMessage);
            _logger.LogInformation("==========================");

            // For production, implement actual email sending here
            // Examples:
            // - SendGrid: https://sendgrid.com/
            // - Mailgun: https://www.mailgun.com/
            // - AWS SES: https://aws.amazon.com/ses/
            // - SMTP: Use System.Net.Mail.SmtpClient

            return Task.CompletedTask;
        }
    }
}

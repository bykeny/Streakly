using HabitGoalTrackerApp.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace HabitGoalTrackerApp.Services.Implementation
{
    /// <summary>
    /// Email sender service that uses SMTP via MailKit when configured,
    /// or falls back to console logging for development.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly EmailSettings _emailSettings;

        public EmailSender(ILogger<EmailSender> logger, EmailSettings emailSettings)
        {
            _logger = logger;
            _emailSettings = emailSettings;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (_emailSettings.IsConfigured)
            {
                // Production mode: Send real email via SMTP
                await SendSmtpEmailAsync(email, subject, htmlMessage);
            }
            else
            {
                // Development mode: Log email to console
                LogEmailToConsole(email, subject, htmlMessage);
            }
        }

        private async Task SendSmtpEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
                message.To.Add(new MailboxAddress(toEmail, toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlMessage
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                
                // Connect to SMTP server
                var secureSocketOptions = _emailSettings.UseSsl 
                    ? SecureSocketOptions.StartTls 
                    : SecureSocketOptions.None;
                    
                await client.ConnectAsync(
                    _emailSettings.SmtpHost, 
                    _emailSettings.SmtpPort, 
                    secureSocketOptions);

                // Authenticate only if credentials are provided
                if (_emailSettings.HasCredentials)
                {
                    await client.AuthenticateAsync(
                        _emailSettings.SmtpUsername, 
                        _emailSettings.SmtpPassword);
                }

                // Send email
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw;
            }
        }

        private void LogEmailToConsole(string email, string subject, string htmlMessage)
        {
            _logger.LogWarning("=== EMAIL (Development Mode - SMTP not configured) ===");
            _logger.LogInformation("To: {Email}", email);
            _logger.LogInformation("Subject: {Subject}", subject);
            _logger.LogInformation("Body: {HtmlMessage}", htmlMessage);
            _logger.LogWarning("=======================================================");
            _logger.LogWarning("To send real emails, configure EmailSettings in appsettings.json or environment variables.");
        }
    }
}

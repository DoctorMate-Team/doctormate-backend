using doctor.Core.Services.contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace doctor.Service.Services
{
    /// <summary>
    /// Email service implementation using SMTP
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Sends a verification email using SMTP
        /// </summary>
        public async Task SendVerificationAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                _logger.LogInformation("Starting to send email to {RecipientEmail}", recipientEmail);

                // Get SMTP settings from configuration
                var smtpHost = _configuration["SmtpSettings:Host"];
                var smtpPort = int.Parse(_configuration["SmtpSettings:Port"] ?? "587");
                var enableSsl = bool.Parse(_configuration["SmtpSettings:EnableSsl"] ?? "true");
                var userName = _configuration["SmtpSettings:UserName"];
                var password = _configuration["SmtpSettings:Password"];
                var senderEmail = _configuration["SmtpSettings:SenderEmail"];
                var senderName = _configuration["SmtpSettings:SenderName"] ?? "Doctor Mate";

                _logger.LogDebug("SMTP Settings - Host: {Host}, Port: {Port}, EnableSsl: {EnableSsl}", 
                    smtpHost, smtpPort, enableSsl);

                // Create SMTP client
                using var smtpClient = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = enableSsl,
                    Credentials = new NetworkCredential(userName, password),
                    Timeout = 30000 // 30 seconds timeout
                };

                _logger.LogDebug("SMTP client created successfully");

                // Create email message
                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail ?? userName!, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(recipientEmail);

                _logger.LogInformation("Sending email via SMTP...");

                // Send email
                await smtpClient.SendMailAsync(mailMessage);

                _logger.LogInformation("Email sent successfully to {RecipientEmail}", recipientEmail);
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, "SMTP error while sending email to {RecipientEmail}. Status: {StatusCode}", 
                    recipientEmail, smtpEx.StatusCode);
                throw new InvalidOperationException($"SMTP error: {smtpEx.Message}", smtpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending email to {RecipientEmail}", recipientEmail);
                throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
            }
        }
    }
}

namespace doctor.Core.Services.contract
{
    /// <summary>
    /// Interface for email service operations
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends a verification email to the recipient
        /// </summary>
        /// <param name="recipientEmail">The recipient's email address</param>
        /// <param name="subject">The email subject</param>
        /// <param name="body">The email body content</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task SendVerificationAsync(string recipientEmail, string subject, string body);
    }
}

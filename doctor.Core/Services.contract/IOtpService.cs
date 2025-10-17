namespace doctor.Core.Services.contract
{
    /// <summary>
    /// Interface for OTP operations
    /// </summary>
    public interface IOtpService
    {
        /// <summary>
        /// Generates and sends an OTP to the user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="recipientEmail">Recipient's email address</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task GenerateAndSendOtpAsync(string userId, string recipientEmail);

        /// <summary>
        /// Verifies an OTP code
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="otpCode">The OTP code to verify</param>
        /// <returns>True if valid, false otherwise</returns>
        Task<bool> VerifyOtpAsync(string userId, string otpCode);

        /// <summary>
        /// Cleans up expired OTP entries
        /// </summary>
        /// <returns>Number of entries removed</returns>
        Task<int> CleanupExpiredOtpsAsync();
    }
}

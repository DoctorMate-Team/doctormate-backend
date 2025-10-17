using doctor.Core.Entities.Otp;
using doctor.Core.Services.contract;
using doctor.Repository.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace doctor.Service.Services
{
    /// <summary>
    /// OTP service implementation
    /// </summary>
    public class OtpService : IOtpService
    {
        private readonly OtpDbContext _otpDbContext;
        private readonly IEmailService _emailService;
        private readonly ILogger<OtpService> _logger;
        private const int OTP_LENGTH = 6;
        private const int OTP_EXPIRY_MINUTES = 10;

        public OtpService(
            OtpDbContext otpDbContext,
            IEmailService emailService,
            ILogger<OtpService> logger)
        {
            _otpDbContext = otpDbContext;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Generates a random OTP code
        /// </summary>
        private string GenerateOtpCode()
        {
            var randomNumber = RandomNumberGenerator.GetInt32(0, (int)Math.Pow(10, OTP_LENGTH));
            return randomNumber.ToString($"D{OTP_LENGTH}");
        }

        /// <summary>
        /// Generates and sends an OTP to the user
        /// </summary>
        public async Task GenerateAndSendOtpAsync(string userId, string recipientEmail)
        {
            try
            {
                _logger.LogInformation("Generating OTP for user {UserId}", userId);

                // Generate OTP code
                var otpCode = GenerateOtpCode();

                // Create OTP entry
                var otpEntry = new OtpEntry
                {
                    UserId = userId,
                    OtpCode = otpCode,
                    RecipientEmail = recipientEmail,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(OTP_EXPIRY_MINUTES),
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow
                };

                // Save to database
                _otpDbContext.OtpEntries.Add(otpEntry);
                await _otpDbContext.SaveChangesAsync();

                _logger.LogInformation("OTP saved to database for user {UserId}", userId);

                // Prepare email content
                var subject = "Your Verification Code - Doctor Mate";
                var body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #2c3e50;'>Verification Code</h2>
                            <p>Hello,</p>
                            <p>Your verification code is:</p>
                            <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; text-align: center;'>
                                <h1 style='color: #007bff; letter-spacing: 5px; margin: 0;'>{otpCode}</h1>
                            </div>
                            <p style='color: #6c757d; font-size: 14px; margin-top: 20px;'>
                                This code will expire in {OTP_EXPIRY_MINUTES} minutes.
                            </p>
                            <p style='color: #6c757d; font-size: 14px;'>
                                If you didn't request this code, please ignore this email.
                            </p>
                            <hr style='border: 1px solid #dee2e6; margin: 20px 0;'>
                            <p style='color: #6c757d; font-size: 12px;'>
                                Best regards,<br>
                                Doctor Mate Team
                            </p>
                        </div>
                    </body>
                    </html>
                ";

                // Send email
                await _emailService.SendVerificationAsync(recipientEmail, subject, body);

                _logger.LogInformation("OTP sent successfully to {Email}", recipientEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating and sending OTP for user {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Verifies an OTP code
        /// </summary>
        public async Task<bool> VerifyOtpAsync(string userId, string otpCode)
        {
            try
            {
                _logger.LogInformation("Verifying OTP for user {UserId}", userId);

                // Find the OTP entry
                var otpEntry = await _otpDbContext.OtpEntries
                    .Where(o => o.UserId == userId && o.OtpCode == otpCode)
                    .OrderByDescending(o => o.CreatedAt)
                    .FirstOrDefaultAsync();

                // Check if OTP exists
                if (otpEntry == null)
                {
                    _logger.LogWarning("OTP not found for user {UserId}", userId);
                    return false;
                }

                // Check if already used
                if (otpEntry.IsUsed)
                {
                    _logger.LogWarning("OTP already used for user {UserId}", userId);
                    return false;
                }

                // Check if expired
                if (otpEntry.ExpiresAt < DateTime.UtcNow)
                {
                    _logger.LogWarning("OTP expired for user {UserId}", userId);
                    return false;
                }

                // Mark as used
                otpEntry.IsUsed = true;
                await _otpDbContext.SaveChangesAsync();

                _logger.LogInformation("OTP verified successfully for user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for user {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Cleans up expired OTP entries
        /// </summary>
        public async Task<int> CleanupExpiredOtpsAsync()
        {
            try
            {
                var expiredOtps = await _otpDbContext.OtpEntries
                    .Where(o => o.ExpiresAt < DateTime.UtcNow || o.IsUsed)
                    .ToListAsync();

                if (expiredOtps.Any())
                {
                    _otpDbContext.OtpEntries.RemoveRange(expiredOtps);
                    await _otpDbContext.SaveChangesAsync();
                    
                    _logger.LogInformation("Cleaned up {Count} expired OTP entries", expiredOtps.Count);
                    return expiredOtps.Count;
                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired OTPs");
                throw;
            }
        }
    }
}

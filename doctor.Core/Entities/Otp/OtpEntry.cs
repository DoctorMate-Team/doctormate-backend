using System.ComponentModel.DataAnnotations;

namespace doctor.Core.Entities.Otp
{
    /// <summary>
    /// OTP Entry entity for storing verification codes
    /// </summary>
    public class OtpEntry : BaseEntity<Guid>
    {
        /// <summary>
        /// User identifier (can be user ID, email, or phone number)
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// The OTP code
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string OtpCode { get; set; } = string.Empty;

        /// <summary>
        /// When the OTP expires
        /// </summary>
        [Required]
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Flag indicating if the OTP has been used
        /// </summary>
        [Required]
        public bool IsUsed { get; set; } = false;

        /// <summary>
        /// When the OTP was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Optional: Store recipient email for reference
        /// </summary>
        [MaxLength(256)]
        public string? RecipientEmail { get; set; }
    }
}

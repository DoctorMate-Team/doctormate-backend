using doctor.Core.Services.contract;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace doctor.APIs.Controllers
{
    /// <summary>
    /// Controller for OTP operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OtpController : ControllerBase
    {
        private readonly IOtpService _otpService;
        private readonly ILogger<OtpController> _logger;

        public OtpController(IOtpService otpService, ILogger<OtpController> logger)
        {
            _otpService = otpService;
            _logger = logger;
        }

        /// <summary>
        /// Send OTP to user's email
        /// </summary>
        [HttpPost("send")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Invalid request data",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }

                await _otpService.GenerateAndSendOtpAsync(request.UserId, request.Email);

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "OTP sent successfully to your email"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending OTP for user {UserId}", request.UserId);
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Failed to send OTP. Please try again later."
                });
            }
        }

        /// <summary>
        /// Verify OTP code
        /// </summary>
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Invalid request data",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }

                var isValid = await _otpService.VerifyOtpAsync(request.UserId, request.OtpCode);

                if (isValid)
                {
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "OTP verified successfully"
                    });
                }
                else
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Invalid or expired OTP code"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for user {UserId}", request.UserId);
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Failed to verify OTP. Please try again later."
                });
            }
        }

        /// <summary>
        /// Cleanup expired OTP entries (Admin endpoint)
        /// </summary>
        [HttpPost("cleanup")]
        public async Task<IActionResult> CleanupExpiredOtps()
        {
            try
            {
                var count = await _otpService.CleanupExpiredOtpsAsync();

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = $"Cleaned up {count} expired OTP entries"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired OTPs");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Failed to cleanup expired OTPs"
                });
            }
        }
    }

    #region Request/Response Models

    /// <summary>
    /// Request model for sending OTP
    /// </summary>
    public class SendOtpRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request model for verifying OTP
    /// </summary>
    public class VerifyOtpRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP code is required")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "OTP code must be between 4 and 10 characters")]
        public string OtpCode { get; set; } = string.Empty;
    }

    /// <summary>
    /// Standard API response model
    /// </summary>
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }
    }

    #endregion
}

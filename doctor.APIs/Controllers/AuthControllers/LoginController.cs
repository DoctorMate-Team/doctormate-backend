using BCrypt.Net;
using doctor.APIs.Models;
using doctor.Core.Entities.Identity;
using doctor.Repository.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace doctor.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DoctorMateDbContext _context;
        private readonly IConfiguration _config;

        public LoginController(DoctorMateDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost]
        [EnableRateLimiting("LoginPolicy")]
        public IActionResult Login([FromBody] LoginDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.EmailOrPhone) || string.IsNullOrEmpty(model.Password))
                    return BadRequest(new ApiResponse(false, "Email/Phone and Password are required."));

                bool isEmail = IsValidEmail(model.EmailOrPhone);

                var user = _context.Users.FirstOrDefault(u =>
                    (isEmail && u.Email == model.EmailOrPhone) || (!isEmail && u.PhoneNumber == model.EmailOrPhone));

                if (user == null)
                    return Unauthorized(new ApiResponse(false, "User not found."));

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
                if (!isPasswordValid)
                    return Unauthorized(new ApiResponse(false, "Password incorrect."));

                var token = GenerateJwtToken(user);

                return Ok(new ApiResponse(true, "Login successful", new
                {
                    Token = token,
                    User = new
                    {
                        user.Id,
                        user.FullName,
                        user.Email,
                        user.Role
                    }
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(false, "An unexpected error occurred.", ex.Message));
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("PhoneNumber", user.PhoneNumber ?? "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginDto
    {
        public string EmailOrPhone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

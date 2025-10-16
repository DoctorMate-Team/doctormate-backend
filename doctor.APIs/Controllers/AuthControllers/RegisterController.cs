//using BCrypt.Net;
//using doctor.Core.Entities;
//using doctor.Core.Entities.Identity;
//using doctor.Repository.Data.Contexts;
//using Microsoft.AspNetCore.Mvc;

//namespace doctor.APIs.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class RegisterController : ControllerBase
//    {
//        private readonly DoctorMateDbContext _context;

//        public RegisterController(DoctorMateDbContext context)
//        {
//            _context = context;
//        }

//        [HttpPost]
//        public IActionResult Register([FromBody] User user)
//        {
//            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.PasswordHash) ||
//                string.IsNullOrEmpty(user.Role) || string.IsNullOrEmpty(user.PhoneNumber))
//            {
//                return BadRequest("Email, phone, password, and role are required.");
//            }

//            if (user.Role != "Patient" && user.Role != "Doctor")
//            {
//                return BadRequest("Role must be either 'Patient' or 'Doctor'.");
//            }

//            bool emailExists = _context.Users.Any(u => u.Email == user.Email);
//            if (emailExists)
//            {
//                return Conflict("Email already exists.");
//            }

//            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
//            user.CreatedAt = DateTime.UtcNow;

//            _context.Users.Add(user);
//            _context.SaveChanges();

//            if (user.Role == "Doctor")
//            {
//                _context.Doctors.Add(new Doctor { UserId = user.Id, CreatedAt = DateTime.UtcNow });
//            }
//            else if (user.Role == "Patient")
//            {
//                _context.Patients.Add(new Patient { UserId = user.Id, CreatedAt = DateTime.UtcNow });
//            }

//            _context.SaveChanges();

//            return Created($"api/register/{user.Id}", new
//            {
//                Message = "User registered successfully",
//                user.Id,
//                user.Email,
//                user.Role
//            });
//        }

//        [HttpPut("completeprofile/{id:guid}")]
//        public IActionResult CompleteProfile(Guid id, [FromBody] object profileData)
//        {
//            var user = _context.Users.FirstOrDefault(u => u.Id == id);
//            if (user == null)
//                return NotFound("User not found.");

//            if (user.Role == "Doctor")
//            {
//                var doctor = _context.Doctors.FirstOrDefault(d => d.UserId == id);
//                if (doctor == null) return NotFound("Doctor not found.");

//                _context.SaveChanges();
//                return Ok("Doctor profile completed successfully.");
//            }
//            else if (user.Role == "Patient")
//            {
//                var patient = _context.Patients.FirstOrDefault(p => p.UserId == id);
//                if (patient == null) return NotFound("Patient not found.");

//                _context.SaveChanges();
//                return Ok("Patient profile completed successfully.");
//            }

//            return BadRequest("Invalid user role.");
//        }

//        [HttpPost("resetpassword")]
//        public IActionResult ResetPassword([FromBody] ResetPasswordDto model)
//        {
//            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
//            if (user == null)
//                return NotFound("User not found.");

//            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
//            _context.SaveChanges();

//            return Ok("Password reset successfully.");
//        }
//    }

//    public class ResetPasswordDto
//    {
//        public string Email { get; set; } = string.Empty;
//        public string NewPassword { get; set; } = string.Empty;
//    }
//}
using BCrypt.Net;
using doctor.Core.Entities;
using doctor.Core.Entities.Identity;
using doctor.Repository.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace doctor.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly DoctorMateDbContext _context;

        public RegisterController(DoctorMateDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Register([FromBody] RegisterDto model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password) ||
                string.IsNullOrEmpty(model.Role) || string.IsNullOrEmpty(model.PhoneNumber))
            {
                return BadRequest("Email, phone, password, and role are required.");
            }

            if (model.Role != "Patient" && model.Role != "Doctor")
            {
                return BadRequest("Role must be either 'Patient' or 'Doctor'.");
            }

            bool emailExists = _context.Users.Any(u => u.Email == model.Email);
            if (emailExists)
            {
                return Conflict("Email already exists.");
            }

            var user = new User
            {
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Role = model.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            if (user.Role == "Doctor")
            {
                _context.Doctors.Add(new Doctor { UserId = user.Id, CreatedAt = DateTime.UtcNow });
            }
            else if (user.Role == "Patient")
            {
                _context.Patients.Add(new Patient { UserId = user.Id, CreatedAt = DateTime.UtcNow });
            }

            _context.SaveChanges();

            return Created($"api/register/{user.Id}", new
            {
                Message = "User registered successfully",
                user.Id,
                user.Email,
                user.Role
            });
        }

        [HttpPut("completeprofile/{id:guid}")]
        public IActionResult CompleteProfile(Guid id, [FromBody] object profileData)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound("User not found.");

            if (user.Role == "Doctor")
            {
                var doctor = _context.Doctors.FirstOrDefault(d => d.UserId == id);
                if (doctor == null) return NotFound("Doctor not found.");

                _context.SaveChanges();
                return Ok("Doctor profile completed successfully.");
            }
            else if (user.Role == "Patient")
            {
                var patient = _context.Patients.FirstOrDefault(p => p.UserId == id);
                if (patient == null) return NotFound("Patient not found.");

                _context.SaveChanges();
                return Ok("Patient profile completed successfully.");
            }

            return BadRequest("Invalid user role.");
        }

        [HttpPost("resetpassword")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
                return NotFound("User not found.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _context.SaveChanges();

            return Ok("Password reset successfully.");
        }
    }

    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? FullName { get; set; }
    }

    public class ResetPasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}


using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BECamp_T13_HW2_Aspnet_AI.Data;
using BECamp_T13_HW2_Aspnet_AI.Models;
using BECamp_T13_HW2_Aspnet_AI.Services;
using Mysqlx.Session;

namespace BECamp_T13_HW2_Aspnet_AI.Controllers
{
    [ApiController]
    [Route("/")]
    public class UserController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IEmailSender _emailSender;

        public UserController(UserContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Register request)
        {
            if (_context.Users.Any(u => u.Email == request.Email))
            {
                return Conflict(new { Response = "User already existed." });
            }

            CreatePasswordHash(request.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt);

            User user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                VerificationToken = CreateRandomToken()
            };

            _context.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Register), new { Response = "User successfully created." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Login request)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return Unauthorized(new { Response = "Login failed. Usernam is not exist." });
            }

            if (user.VerifiedAt == null)
            {
                return Unauthorized(new { Response = "User not verified." });
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Unauthorized(new { Response = "Login failed. Password is incorrect." });
            }

            return Ok(new { Response = "Login success." });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify(string verificationToken)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == verificationToken);

            if (user == null)
            {
                return Unauthorized(new { Response = "Invalid token." });
            }

            user.VerifiedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(new { Response = "User verified." });
        }

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return BadRequest(new { Reponse = "User not found." });
            }

            user.PasswordResetToken = CreateRandomToken();
            user.ResetTokenExpires = DateTime.Now.AddDays(1);
            await _context.SaveChangesAsync();

            return Ok(new { Response = "You may reset your password." });
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPassword request)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.PasswordResetToken);

            if (user == null || user.ResetTokenExpires < DateTime.Now)
            {
                return BadRequest(new { Reponse = "Invalid Token." });
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            await _context.SaveChangesAsync();

            return Ok(new { Response = "Password successfully reset." });
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (HMACSHA512 hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (HMACSHA512 hmac = new HMACSHA512(passwordSalt))
            {
                byte[] computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                return computeHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

    }
}
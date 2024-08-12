using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using BECamp_T13_HW2_Aspnet_AI.Data;
using BECamp_T13_HW2_Aspnet_AI.Models;
using Microsoft.EntityFrameworkCore;

namespace BECamp_T13_HW2_Aspnet_AI.Controllers
{
    [ApiController]
    [Route("/")]
    public class UserController : ControllerBase
    {
        private readonly UserContext _context;

        public UserController(UserContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Login login)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);

            if (user == null)
            {
                return Unauthorized(new { Response = "Login failed. Usernam is not exist." });
            }

            if (user.VerifiedAt == null)
            {
                return Unauthorized(new { Response = "User not verified." });
            }

            if (!VerifyPasswordHash(login.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Unauthorized(new { Response = "Login failed. Password is incorrect." });
            }

            return Ok(new { Response = "Login success." });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Register register)
        {
            if (_context.Users.Any(u => u.Email == register.Email))
            {
                return Conflict(new { Response = "User already existed." });
            }

            CreatePasswordHash(register.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt);

            User user = new User
            {
                Email = register.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                VerificationToken = CreateVerificationToken()
            };

            _context.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Register), new { Response = "User successfully created." });
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

        private string CreateVerificationToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

    }
}
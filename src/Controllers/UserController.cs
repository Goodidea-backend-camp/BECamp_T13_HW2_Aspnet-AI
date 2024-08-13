using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BECamp_T13_HW2_Aspnet_AI.Data;
using BECamp_T13_HW2_Aspnet_AI.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace BECamp_T13_HW2_Aspnet_AI.Controllers
{
    [ApiController]
    [Route("/")]
    public class UserController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IConfiguration _configuration;

        public UserController(UserContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

            string jwtToken = CreateJwtToken(user);

            RefreshToken refreshToken = CreateRefreshToken();
            SetRefreshToken(user, refreshToken);
            
            await _context.SaveChangesAsync();

            return Ok(jwtToken);
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

        [HttpPost("forgot-password")]
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

        [HttpPost("reset-password")]
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

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            string refreshToken = Request.Cookies["refreshToken"];

            User user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null)
            {
                return Unauthorized("Invalid refresh token.");
            }

            if (user.ResetTokenExpires < DateTime.Now)
            {
                return Unauthorized("Refresh token expired.");
            }

            string jwtToken = CreateJwtToken(user);

            RefreshToken newRefreshToken = CreateRefreshToken();
            SetRefreshToken(user, newRefreshToken);

            await _context.SaveChangesAsync();

            return Ok(jwtToken);
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

        private string CreateJwtToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email)
            };

            SymmetricSecurityKey singInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("JWT:Token").Value));

            SigningCredentials credentials = new SigningCredentials(singInKey, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private RefreshToken CreateRefreshToken()
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                TokenCreated = DateTime.Now,
                TokenExpires = DateTime.Now.AddDays(1)
            };
        }

        private void SetRefreshToken(User user, RefreshToken newRefreshToken)
        {
            CookieOptions cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.TokenExpires
            };

            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            user.RefreshToken = newRefreshToken.Token;
            user.RefreshTokenCreated = newRefreshToken.TokenCreated;
            user.RefreshTokenExpires = newRefreshToken.TokenExpires;
        }
    }
}
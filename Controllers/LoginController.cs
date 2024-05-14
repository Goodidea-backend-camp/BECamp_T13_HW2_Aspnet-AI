using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BECamp_T13_HW2_Aspnet_AI.Data;
using BECamp_T13_HW2_Aspnet_AI.Models;

namespace BECamp_T13_HW2_Aspnet_AI.Controllers
{
    [Route("/")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        readonly LoginContext _logincontext;

        public LoginController(LoginContext context)
        {
            _logincontext = context;
        }

        // POST: api/Login
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("login")]
        public async Task<ActionResult> PostLogin(Login login)
        {
            // Check whether ussername/password is null, empty, or consists only of white-space.
            if (String.IsNullOrWhiteSpace(login.username) || String.IsNullOrWhiteSpace(login.password))
            {
                return Unauthorized();
            }

            try
            {
                List<Login> loginResults = _logincontext.Logins
                    .Where(result => result.username == login.username && result.password == login.password)
                    .ToList();

                if (loginResults.Count() == 0)
                {
                    return Unauthorized();
                }

                return Ok(login);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
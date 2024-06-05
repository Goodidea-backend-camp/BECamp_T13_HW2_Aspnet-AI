using Microsoft.AspNetCore.Mvc;
using BECamp_T13_HW2_Aspnet_AI.Data;
using BECamp_T13_HW2_Aspnet_AI.Models;

namespace BECamp_T13_HW2_Aspnet_AI.Controllers
{
    [Route("/")]
    [ApiController]
    [Produces("application/json")]
    public class LoginController : ControllerBase
    {
        private readonly LoginContext _logincontext;

        public LoginController(LoginContext context)
        {
            _logincontext = context;
        }

        /// <summary>
        /// Login with an exist account with username and password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Success or failed with a message</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /login
        ///     {
        ///        "username": "HelloUsername",
        ///        "password": "HelloPassword"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns a success message</response>
        /// <response code="401">If the value is null, empty, white space, or the account's data is wrong or not exist</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> PostLogin(Login login)
        {
            string loginErrorMessage = "Login failed. Username/Password is not exist or wrong.";
            string loginSuccessMessage = "Login success.";

            // Check whether ussername/password is null, empty, or consists only of white-space.
            if (String.IsNullOrWhiteSpace(login.username) || String.IsNullOrWhiteSpace(login.password))
            {
                return Unauthorized(new {Response = loginErrorMessage});
            }

            try
            {
                List<Login> loginResults = _logincontext.Logins
                    .Where(result => result.username == login.username && result.password == login.password)
                    .ToList();

                if (loginResults.Count() == 0)
                {
                    return Unauthorized(new {Response = loginErrorMessage});
                }

                return Ok(new {Response = loginSuccessMessage});
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
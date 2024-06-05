using Microsoft.AspNetCore.Mvc;
using BECamp_T13_HW2_Aspnet_AI.Data;
using BECamp_T13_HW2_Aspnet_AI.Models;

namespace BECamp_T13_HW2_Aspnet_AI.Controllers
{
    [Route("/")]
    [ApiController]
    [Produces("application/json")]
    public class RegisterController : ControllerBase
    {
        private readonly RegisterContext _registercontext;
        // To record user id that has been used.
        private int _userId = 2;

        public RegisterController(RegisterContext context)
        {
            _registercontext = context;
        }

        /// <summary>
        /// Register a new account with email, username and password.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Success or failed with a message</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /register
        ///     {
        ///        "email": "HelloEmail@mail.com"    
        ///        "username": "HelloUsername",
        ///        "password": "HelloPassword"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns a success message</response>
        /// <response code="401">If the value is null, empty or white space</response>
        /// <response code="409">If the email or username has existed</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<Register>> PostRegister(Register register)
        {
            string registerErrorMessage = "Email/Username/Password is required.";
            string usernameHasExist = "Username has existed, please try another username.";
            string emailHasBeenUsed = "Email has been used, please try another email.";

            if (String.IsNullOrWhiteSpace(register.username) || String.IsNullOrWhiteSpace(register.password)
                || String.IsNullOrEmpty(register.email))
            {
                return Unauthorized(new { Response = registerErrorMessage });
            }

            register.id = ++_userId;

            try
            {
                List<Register> registerUsernameResults = _registercontext.Registers
                    .Where(result => result.username == register.username)
                    .ToList();
                List<Register> registerEmailResults = _registercontext.Registers
                    .Where(result => result.email == register.email)
                    .ToList();

                if (registerUsernameResults.Count > 0)
                {
                    return Conflict(new { Response = usernameHasExist });
                }

                if (registerEmailResults.Count > 0)
                {
                    return Conflict(new { Response = emailHasBeenUsed });
                }

                _registercontext.Registers.Add(register);
                await _registercontext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return CreatedAtAction(nameof(PostRegister), new { id = register.username }, register);
        }
    }
}
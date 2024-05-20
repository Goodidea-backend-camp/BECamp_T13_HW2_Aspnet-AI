using Microsoft.AspNetCore.Mvc;
using BECamp_T13_HW2_Aspnet_AI.Data;
using BECamp_T13_HW2_Aspnet_AI.Models;

namespace BECamp_T13_HW2_Aspnet_AI.Controllers
{
    [Route("/")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        readonly RegisterContext _registercontext;
        // To record user id that has been used.
        int _userId = 1;

        public RegisterController(RegisterContext context)
        {
            _registercontext = context;
        }

        // POST: api/Register
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("register")]
        public async Task<ActionResult<Register>> PostRegister(Register register)
        {
            if (String.IsNullOrWhiteSpace(register.username) || String.IsNullOrWhiteSpace(register.password)
                || String.IsNullOrEmpty(register.email))
            {
                return Unauthorized();
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

                if(registerUsernameResults.Count > 0)
                {
                    return Conflict(new {Response = "username has existed, please try another username"});
                }

                if(registerEmailResults.Count > 0)
                {
                    return Conflict(new {Response = "email has been used, please try another email"});
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
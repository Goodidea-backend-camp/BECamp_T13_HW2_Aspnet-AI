using Microsoft.AspNetCore.Mvc;
using BECamp_T13_HW2_Aspnet_AI.Services;
using System.Text.Json.Nodes;

namespace BECamp_T13_HW2_Aspnet_AI.Controllers
{
    [Route("/")]
    [ApiController]
    [Produces("application/json")]
    public class AIAssistantController : ControllerBase
    {
        private readonly IAIServices _assistant;

        public AIAssistantController(IAIServices assistant)
        {
            _assistant = assistant;
        }

        /// <summary>
        /// Check whether username is spam or not.
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns>Success or failed with a message</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /replies
        ///     {
        ///        "prompt": "Hello, world!"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns a success message with response</response>
        /// <response code="400">If the body or prompt's value is null</response>
        [HttpPost("replies")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UserInputSpamCheck([FromBody] JsonObject body)
        {
            // Get the value from body which content-type is json.
            string prompt = body["prompt"].ToString();
            // Wait for AI check if the nick name user input is spam or not.
            string spamResponse = await _assistant.SpamCheck(prompt);
            // The method automatically tranform the new object into json.
            return Ok(new { Response = spamResponse });
        }

        /// <summary>
        /// Generate an image from user's about me.
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns>Success or failed with a message</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /image
        ///     {
        ///        "prompt": "A handsome man that coding with thinkpad."
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns a success message with response</response>
        /// <response code="400">If the body or prompt's value is null</response>
        [HttpPost("image")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UserInputTextToImage([FromBody] JsonObject body)
        {
            // Get the value from body which content-type is json.
            string prompt = body["prompt"].ToString();
            // Wait for AI response a image uri.
            string imageUriResponse = await _assistant.Visualize(prompt);
            // Return a Uri by json format and let frontend open it with <img src="">.
            return Ok(new { Response = imageUriResponse });
        }

        /// <summary>
        /// Generate a speech mp3 file from user's prompt.
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns>Success or failed with a message</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /roast
        ///     {
        ///        "prompt": "Ruby on Rails"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns a success message with response</response>
        /// <response code="400">If the body or prompt's value is null</response>
        [HttpPost("roast")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UserInputTextToSpeech([FromBody] JsonObject body)
        {
            // Get the value from body which content-type is json.
            string prompt = body["prompt"].ToString();

            // Check the prompt length is required length or not.
            if (prompt.Length < 2 || prompt.Length > 50)
            {
                return BadRequest(new { Response = "Required string length minimum 2 maximum 50" });
            }

            // Then wait for AI generate a speech binary and create a mp3 for writing in and response.
            string pathOfMP3Response = await _assistant.TextToSpeech(prompt);
            // Return a path of mp3 by json format and let frontend open it with <a href="">.
            return Ok(new { Response = pathOfMP3Response });
        }
    }
}
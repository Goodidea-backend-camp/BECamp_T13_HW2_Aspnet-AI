using Microsoft.AspNetCore.Mvc;
using BECamp_T13_HW2_Aspnet_AI.Services;

namespace BECamp_T13_HW2_Aspnet_AI.Controllers
{
    [Route("/")]
    [ApiController]
    public class AIAssistantController : ControllerBase
    {
        readonly IAIServices _assistant;

        public AIAssistantController(IAIServices assistant)
        {
            _assistant = assistant;
        }

        // POST: /replies
        [HttpPost("replies")]
        public async Task<ActionResult> UserInputSpamCheck(string prompt)
        {
            // Wait for AI check if the nick name user input is spam or not.
            string spamResponse = await _assistant.SpamCheck(prompt);
            // The method automatically tranform the new object into json.
            return Ok(new { Response = spamResponse });
        }

        // POST: /image
        [HttpPost("image")]
        public async Task<ActionResult> TextToImage(string prompt)
        {
            // Wait for AI response a image uri.
            string imageUriResponse = await _assistant.Visualize(prompt);
            // Return a Uri by json format and let frontend open it with <img src="">.
            return Ok(new { Response = imageUriResponse });
        }

        // POST: /roast
        [HttpPost("roast")]
        public async Task<ActionResult> TextToSpeech(string prompt)
        {
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
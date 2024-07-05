using Xunit;
using BECamp_T13_HW2_Aspnet_AI.Services;

namespace BECamp_T13_HW2_Aspnet_AI
{
    public class AIService_Should
    {
        private string spamWasDetect = "Spam was detected.";
        private string postIsValid = "Post is valid.";

        [Fact]
        public async void SpamCheck_IsSpamDetect()
        {
            IAIServices services = new OpenAIServices();

            string fuckYouResult = await services.SpamCheck("Fuck You!");
            string helloWorldResult = await services.SpamCheck("Hello, world!");

            Assert.Equal(spamWasDetect, fuckYouResult);
            Assert.Equal(postIsValid, helloWorldResult);
        }
    }
}

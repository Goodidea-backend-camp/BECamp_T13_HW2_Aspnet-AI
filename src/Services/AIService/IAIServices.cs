namespace BECamp_T13_HW2_Aspnet_AI.Services
{
    public interface IAIServices
    {
        internal Task<string> SpamCheck(string prompt);
        internal Task<string> TextToSpeech(string prompt);
        internal Task<string> Visualize(string prompt);
    }
}
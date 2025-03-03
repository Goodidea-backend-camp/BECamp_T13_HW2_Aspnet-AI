using System.Text;
using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using BECamp_T13_HW2_Aspnet_AI.Models;

namespace BECamp_T13_HW2_Aspnet_AI.Services
{
    internal class OpenAIServices : IAIServices
    {
        // To get the OpenAI API key that is stored in the user-secrets list.
        private static IConfigurationRoot userSecretConfig = new ConfigurationBuilder()
                .AddUserSecrets<OpenAIServices>()
                .Build();

        private static string nonAzureOpenAIApiKey = userSecretConfig["OpenAI:APIKey"] ?? throw new InvalidOperationException("'OpenAI API Key' not found.");
        private static StringBuilder imageCompositePrompt = new StringBuilder();

        // The stream chat module using in text generation.
        public async Task<string> StreamChatWithNonAzureOpenAI(OpenAIClient client, string prompt)
        {
            // The configuration information for a chat completions request.
            ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
            {
                DeploymentName = "gpt-3.5-turbo",
                Messages =
                {
                    new ChatRequestUserMessage(prompt)
                }
            };

            // Build a response string.
            StringBuilder response = new StringBuilder();
            // Represent an incremental update to a streamed chat response.
            await foreach (StreamingChatCompletionsUpdate chatUpdate in client.GetChatCompletionsStreaming(chatCompletionsOptions))
            {
                // Composite the response from the OpenAI.
                {
                    response.Append(chatUpdate.ContentUpdate);
                }
            }

            return response.ToString();
        }

        public async Task<string> SpamCheck(string prompt)
        {
            OpenAIClient spamClient = new OpenAIClient(nonAzureOpenAIApiKey, new OpenAIClientOptions());
            // Build the prompt string to ask AI assistant.
            StringBuilder spamPrompt = new StringBuilder("You are a forum moderator who always responds using JSON.");
            spamPrompt.Append("Please inspect the following text and determine if it is spam.");
            spamPrompt.Append(new { prompt }.ToString());
            spamPrompt.Append("Expected Response Example: {\"is_spam\": true|false}");

            string streamChatResponse = await StreamChatWithNonAzureOpenAI(spamClient, spamPrompt.ToString());
            // Parses the text to a single JSON value.
            IsSpam isSpam = JsonSerializer.Deserialize<IsSpam>(streamChatResponse);

            if (isSpam.is_spam == true)
            {
                return "Spam was detected.";
            }
            else
            {
                return "Post is valid.";
            }
        }

        public async Task<string> TextToSpeech(string prompt)
        {
            OpenAIClient speechClient = new OpenAIClient(nonAzureOpenAIApiKey, new OpenAIClientOptions());
            StringBuilder speechPrompt = new StringBuilder($"Please roast {prompt} in a sarcastic tone");
            // Generate the text of the roast first and then generate the speech of it.
            string streamChatResponse = await StreamChatWithNonAzureOpenAI(speechClient, speechPrompt.ToString());
            // The request options that control the behavior of a text-to-speech operation.
            SpeechGenerationOptions speechOptions = new()
            {
                Input = streamChatResponse,
                DeploymentName = "tts-1",
                Voice = SpeechVoice.Alloy,
                ResponseFormat = SpeechGenerationResponseFormat.Mp3,
                Speed = 1.0f
            };
            // The result of a successful text-to-speech generation.
            Response<BinaryData> speechResponse = await speechClient.GenerateSpeechFromTextAsync(speechOptions);

            string pathOfMP3 = $"./assets/{prompt}.mp3";
            File.WriteAllBytes(pathOfMP3, speechResponse.Value.ToArray());

            return pathOfMP3;
        }

        public async Task<string> Visualize(string prompt)
        {
            // Composite the continuous prompt from user.
            imageCompositePrompt.Append(prompt);
            OpenAIClient imageClient = new OpenAIClient(nonAzureOpenAIApiKey, new OpenAIClientOptions());
            // The result of a successful image generation.
            Response<ImageGenerations> imageGenerationResponse = await imageClient.GetImageGenerationsAsync(
            new ImageGenerationOptions()
            {
                DeploymentName = "dall-e-2",
                Prompt = imageCompositePrompt.ToString(),
                Size = ImageSize.Size256x256,
                Quality = ImageGenerationQuality.Standard
            });

            ImageGenerationData generatedImage = imageGenerationResponse.Value.Data[0];

            return generatedImage.Url.AbsoluteUri;
        }
    }
}
using System.Text;
using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using BECamp_T13_HW2_Aspnet_AI.Data;

namespace BECamp_T13_HW2_Aspnet_AI.Services
{
    internal class OpenAIAssistant
    {
        // To get the OpenAI API key that is stored in the user-secrets list.
        static IConfigurationRoot userSecretConfig = new ConfigurationBuilder()
                .AddUserSecrets<OpenAIAssistant>()
                .Build();

        static string nonAzureOpenAIApiKey = userSecretConfig["OpenAI:APIKey"];

        // The stream chat module using in text generation.
        async Task<string> StreamChatWithNonAzureOpenAI(OpenAIClient client, string prompt)
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

        internal async Task<string> SpamCheck(string prompt)
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

        internal async Task<string> Visualize(string prompt)
        {
            OpenAIClient client = new OpenAIClient(nonAzureOpenAIApiKey, new OpenAIClientOptions());
            Response<ImageGenerations> response = await client.GetImageGenerationsAsync(
            new ImageGenerationOptions()
            {
                DeploymentName = "dall-e-2",
                Prompt = prompt,
                Size = ImageSize.Size256x256,
                Quality = ImageGenerationQuality.Standard
            });

            ImageGenerationData generatedImage = response.Value.Data[0];

            return generatedImage.Url.AbsoluteUri;
        }
    }
}
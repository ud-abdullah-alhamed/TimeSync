using RestSharp;
using System.Text.Json;

namespace TimeSync.Services
{
    public class ChatGPTService
    {
        private readonly string _apiKey;

        public ChatGPTService(string apiKey)
        {
            _apiKey = apiKey;
        }


        public async Task<string> GetChatGPTResponse(string message)
        {
            var client = new RestClient("https://api.openai.com/v1/chat/completions");
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Authorization", $"Bearer {_apiKey}");
            request.AddHeader("Content-Type", "application/json");

            var body = new
            {
                model = "gpt-4",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = message }
                },
                max_tokens = 100,
                temperature = 0.7
            };

            request.AddJsonBody(body);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var responseData = JsonSerializer.Deserialize<JsonElement>(response.Content);
                Console.WriteLine(response.Content);
                return responseData.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
            }

            throw new Exception("Failed to get response from ChatGPT: " + response.ErrorMessage);
        }

    }
}

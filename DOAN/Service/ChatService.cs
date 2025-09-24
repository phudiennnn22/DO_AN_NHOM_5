using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DOAN.Respository;
using Microsoft.Extensions.Configuration;

namespace DOAN.Service
{
    public class ChatService : IChatService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;

        public ChatService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"]
                      ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY")
                      ?? string.Empty;
            _model = configuration["Gemini:Model"] ?? "gemini-1.5-flash";
        }

        public async Task<string> GetBotResponseAsync(string message, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                return "Chat hiện chưa được cấu hình khoá Gemini. Vui lòng thêm 'Gemini:ApiKey' (hoặc biến môi trường GEMINI_API_KEY).";
            }

            var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

            var payload = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = message } } }
                }
            };

            var json = JsonSerializer.Serialize(payload);
            using var req = new HttpRequestMessage(HttpMethod.Post, requestUrl)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            req.Headers.Accept.Clear();
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var resp = await _httpClient.SendAsync(req, cancellationToken);
            var respText = await resp.Content.ReadAsStringAsync(cancellationToken);
            if (!resp.IsSuccessStatusCode)
            {
                return $"Gemini error: {resp.StatusCode} - {respText}";
            }

            try
            {
                using var doc = JsonDocument.Parse(respText);
                var root = doc.RootElement;
                var text = root
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();
                return text ?? string.Empty;
            }
            catch
            {
                return respText;
            }
        }
    }
} 
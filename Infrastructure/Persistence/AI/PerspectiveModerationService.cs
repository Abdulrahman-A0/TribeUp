using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServiceAbstraction.Contracts;
using ServiceAbstraction.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Persistence.AI
{
    public class PerspectiveModerationService : IContentModerationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _endpoint;

        public PerspectiveModerationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["PerspectiveApi:ApiKey"]!;
            _endpoint = configuration["PerspectiveApi:Endpoint"]!;
        }

        public async Task<ModerationResult> AnalyzeAsync(string _text)
        {
            var request = new
            {
                comment = new
                {
                    text = _text
                },
                languages = new[] { "en" },
                requestedAttributes = new
                {
                    TOXICITY = new { },
                    INSULT = new { },
                    THREAT = new { },
                    SEXUALLY_EXPLICIT = new { }
                }
            };

            string url = $"{_endpoint}?key={_apiKey}";

            string json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(jsonString);

            var toxicity = document
                .RootElement
                .GetProperty("attributeScores")
                .GetProperty("TOXICITY")
                .GetProperty("summaryScore")
                .GetProperty("value")
                .GetDouble();

            bool denied = toxicity >= 0.7;

            return new ModerationResult
            {
                IsAccepted = !denied,
                DetectedIssue = denied ? "TOXICITY" : "NONE",
                ConfidenceScore = toxicity
            };
        }
    }
}

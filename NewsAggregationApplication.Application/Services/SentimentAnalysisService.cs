using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.UI.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NewsAggregationApplication.UI.Services;

public class SentimentAnalysisService:ISentimentAnalysisService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SentimentAnalysisService> _logger;
    private readonly string _endpoint;
    private readonly string _apiKey;

    public SentimentAnalysisService(HttpClient httpClient, IConfiguration configuration, ILogger<SentimentAnalysisService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _endpoint = configuration["AzureCognitiveServices:Endpoint"];
        _apiKey = configuration["AzureCognitiveServices:ApiKey"];
    }
    private IEnumerable<string> SplitText(string text, int maxLength)
    {
        for (int i = 0; i < text.Length; i += maxLength)
        {
            yield return text.Substring(i, Math.Min(maxLength, text.Length - i));
        }
    }

    public async Task<float> AnalyzeSentimentAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            _logger.LogWarning("Empty content provided for sentiment analysis.");
            return 0.5f; // Neutral score for empty content
        }

        const int maxTextLength = 5120;
        var textChunks = SplitText(text, maxTextLength).ToList();
        var overallScore = 0f;
        var chunkCount = 0;

        foreach (var chunk in textChunks)
        {
            var documents = new { documents = new[] { new { id = "1", text = chunk } } };
            var content = new StringContent(JsonConvert.SerializeObject(documents), Encoding.UTF8, "application/json");
            content.Headers.Add("Ocp-Apim-Subscription-Key", _apiKey);
            
            var response = await _httpClient.PostAsync($"{_endpoint}/text/analytics/v3.1/sentiment", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error analyzing sentiment: {response.ReasonPhrase}");
                throw new Exception("Error analyzing sentiment");
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<JObject>(responseBody);
            var document = result["documents"]?.FirstOrDefault();
            if (document == null)
            {
                _logger.LogWarning("No documents found in sentiment analysis response");
                return 0.5f; // Default to a neutral score if analysis fails
            }

            overallScore += (float)document["confidenceScores"]["positive"];
            chunkCount++;
        }

        return chunkCount > 0 ? overallScore / chunkCount : 0.5f;
    }
}
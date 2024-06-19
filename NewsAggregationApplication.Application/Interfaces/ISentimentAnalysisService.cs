namespace NewsAggregationApplication.UI.Interfaces;

public interface ISentimentAnalysisService
{
    public Task<float> AnalyzeSentimentAsync(string text);
}
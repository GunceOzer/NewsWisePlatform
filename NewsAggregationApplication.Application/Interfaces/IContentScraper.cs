namespace NewsAggregationApplication.UI.Interfaces;

public interface IContentScraper
{
    public Task<string> ScrapeWebPage(string url);
}
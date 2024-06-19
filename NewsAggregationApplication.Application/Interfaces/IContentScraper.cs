using System.ServiceModel.Syndication;

namespace NewsAggregationApplication.UI.Interfaces;

public interface IContentScraper
{
    public Task<string> ScrapeWebPage(string url);
    public string CleanDescription(SyndicationItem item);
}
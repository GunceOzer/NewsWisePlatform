using System.ServiceModel.Syndication;

namespace NewsAggregationApplication.UI.Interfaces;

public interface IImageExtractor
{
    public Task<string> ExtractImageUrl(SyndicationItem item);
}
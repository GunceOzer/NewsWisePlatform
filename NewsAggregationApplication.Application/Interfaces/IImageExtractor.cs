using System.ServiceModel.Syndication;

namespace NewsAggregationApplication.UI.Interfaces;

public interface IImageExtractor
{
    public string ExtractImageUrl(SyndicationItem item);
}
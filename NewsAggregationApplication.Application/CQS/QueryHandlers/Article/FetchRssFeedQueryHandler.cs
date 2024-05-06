using System.ServiceModel.Syndication;
using System.Xml;
using MediatR;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.UI.Queries.Article;

namespace NewsAggregationApplication.UI.CQS.QueryHandlers;

public class FetchRssFeedQueryHandler:IRequestHandler<FetchRssFeedQuery,SyndicationFeed>
{
    private readonly ILogger<FetchRssFeedQueryHandler> _logger;

    public FetchRssFeedQueryHandler(ILogger<FetchRssFeedQueryHandler> logger)
    {
        _logger = logger;
    }

    public async Task<SyndicationFeed> Handle(FetchRssFeedQuery query, CancellationToken cancellationToken)
    {
        try
        {
            using var reader = XmlReader.Create(query.Url);
            var feed = SyndicationFeed.Load(reader);
            _logger.LogInformation($"RSS Feed successfully fetched from {query.Url}");
            return feed;
        }
        catch (XmlException ex)
        {
            _logger.LogError(ex, $"Error parsing RSS Feed from {query.Url}");
            return null; // Depending on your error handling policy, you might want to return null or a default instance instead of throwing
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching RSS Feed from {query.Url}");
            return null;
        }
        
    }
}
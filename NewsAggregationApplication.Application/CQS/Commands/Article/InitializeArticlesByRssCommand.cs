using System.ServiceModel.Syndication;
using MediatR;

namespace NewsAggregationApplication.UI.Commands;

public class InitializeArticlesByRssCommand:IRequest
{
    public IEnumerable<SyndicationItem> RssData { get; set; }
    public Guid SourceId { get; set; }
}
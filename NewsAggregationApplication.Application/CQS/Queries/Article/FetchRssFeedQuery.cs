using System.ServiceModel.Syndication;
using MediatR;

namespace NewsAggregationApplication.UI.Queries.Article;


public class FetchRssFeedQuery:IRequest<SyndicationFeed>
{
    public string Url { get; set; }
}
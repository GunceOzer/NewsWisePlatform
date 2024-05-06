using System.ServiceModel.Syndication;
using MediatR;
namespace NewsAggregationApplication.UI.Commands.Article;

public class ExtractImageCommand:IRequest
{
    public Guid ArticleId { get; set; }
    public SyndicationItem Item { get; set; }

}
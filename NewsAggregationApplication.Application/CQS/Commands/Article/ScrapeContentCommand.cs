using MediatR;

namespace NewsAggregationApplication.UI.Commands.Article;

public class ScrapeContentCommand:IRequest
{
    public Guid ArticleId { get; set; }
    public string Url { get; set; }
}
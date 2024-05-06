using MediatR;

namespace NewsAggregationApplication.UI.Commands.Article;

public class DeleteArticleCommand:IRequest<bool>
{
    public Guid ArticleId { get; set; }
    
}
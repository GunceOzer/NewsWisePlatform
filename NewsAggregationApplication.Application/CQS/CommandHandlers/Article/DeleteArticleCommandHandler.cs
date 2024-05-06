using MediatR;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.Commands.Article;

namespace NewsAggregationApplication.UI.CommandHandlers.Article;

public class DeleteArticleCommandHandler: IRequestHandler<DeleteArticleCommand,bool>
{
    
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<DeleteArticleCommandHandler> _logger;

    public DeleteArticleCommandHandler(NewsDbContext dbContext, ILogger<DeleteArticleCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await _dbContext.Articles.FindAsync(request.ArticleId);
        if (article == null)
        {
            _logger.LogWarning($"Article with ID: {request.ArticleId} not found for deletion.");
            return false;
        }

        _dbContext.Articles.Remove(article);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Article with ID: {request.ArticleId} successfully deleted.");
        return true;
    }
}
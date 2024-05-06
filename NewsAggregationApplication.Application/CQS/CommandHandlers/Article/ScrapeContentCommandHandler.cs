using MediatR;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.Commands.Article;
using NewsAggregationApplication.UI.Interfaces;

namespace NewsAggregationApplication.UI.CommandHandlers.Article;

public class ScrapeContentCommandHandler: IRequestHandler<ScrapeContentCommand>
{
    
    private readonly IContentScraper _contentScraper;
    private readonly ILogger<ScrapeContentCommandHandler> _logger;
    private readonly NewsDbContext _dbContext;

    public ScrapeContentCommandHandler(IContentScraper contentScraper, NewsDbContext dbContext, ILogger<ScrapeContentCommandHandler> logger)
    {
        _contentScraper = contentScraper;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(ScrapeContentCommand command, CancellationToken cancellationToken)
    {
        var content = await _contentScraper.ScrapeWebPage(command.Url);
        if (string.IsNullOrEmpty(content))
        {
            _logger.LogWarning($"No content found at URL: {command.Url}");
            content = "Content not available"; // Default content
        }
        var article = await _dbContext.Articles.FindAsync(command.ArticleId);
        if (article != null)
        {
            article.Content = content;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        
    }
}
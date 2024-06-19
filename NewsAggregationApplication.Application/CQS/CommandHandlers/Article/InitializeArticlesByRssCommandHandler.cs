using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.Commands;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Queries.Article;

namespace NewsAggregationApplication.UI.CommandHandlers.Article;

public class InitializeArticlesByRssCommandHandler : IRequestHandler<InitializeArticlesByRssCommand>
{
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<InitializeArticlesByRssCommandHandler> _logger;
    private readonly IContentScraper _contentScraper;
    private readonly IImageExtractor _imageExtractor;
    private readonly ISentimentAnalysisService _sentimentAnalysisService;


    public InitializeArticlesByRssCommandHandler(NewsDbContext dbContext,
        ILogger<InitializeArticlesByRssCommandHandler> logger, IContentScraper contentScraper,
        IImageExtractor imageExtractor, ISentimentAnalysisService sentimentAnalysisService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _contentScraper = contentScraper;
        _imageExtractor = imageExtractor;
        _sentimentAnalysisService = sentimentAnalysisService;
    }

    public async Task Handle(InitializeArticlesByRssCommand command, CancellationToken cancellationToken)
    {
        foreach (var item in command.RssData)
        {
            var articleUrl = item.Links.FirstOrDefault()?.Uri.ToString();
            if (await _dbContext.Articles.AnyAsync(a => a.SourceUrl == articleUrl, cancellationToken))
            {
                _logger.LogInformation($"Skipped addition of duplicate article from URL: {articleUrl}");
                continue;
            }

            var article = new Data.Entities.Article
            {
                Id = Guid.NewGuid(),
                Title = item.Title.Text,
                Description = _contentScraper.CleanDescription(item),
                Content = await _contentScraper.ScrapeWebPage(articleUrl),
                UrlToImage = await _imageExtractor.ExtractImageUrl(item),
                PublishedDate = item.PublishDate.UtcDateTime,
                SourceUrl = articleUrl,
                SourceId = command.SourceId
            };

            // Calculate the positivity score
            article.PositivityScore = await _sentimentAnalysisService.AnalyzeSentimentAsync(article.Content);

            _dbContext.Articles.Add(article);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Articles initialized with content and images.");
    }
}
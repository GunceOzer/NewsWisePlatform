using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.Commands;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Queries.Article;

namespace NewsAggregationApplication.UI.CommandHandlers.Article;

public class InitializeArticlesByRssCommandHandler : IRequestHandler<InitializeArticlesByRssCommand>
{
    //to create a new DbContext instance for each operation
    private readonly IServiceProvider _serviceProvider;
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<InitializeArticlesByRssCommandHandler> _logger;
    private readonly IContentScraper _contentScraper;
    private readonly IImageExtractor _imageExtractor;
    private readonly ISentimentAnalysisService _sentimentAnalysisService;


    public InitializeArticlesByRssCommandHandler(NewsDbContext dbContext,
        ILogger<InitializeArticlesByRssCommandHandler> logger, IContentScraper contentScraper,
        IImageExtractor imageExtractor, ISentimentAnalysisService sentimentAnalysisService, IServiceProvider serviceProvider)
    {
        _dbContext = dbContext;
        _logger = logger;
        _contentScraper = contentScraper;
        _imageExtractor = imageExtractor;
        _sentimentAnalysisService = sentimentAnalysisService;
        _serviceProvider = serviceProvider;
    }

    public async Task Handle(InitializeArticlesByRssCommand command, CancellationToken cancellationToken)
    {
        await Parallel.ForEachAsync(command.RssData, new ParallelOptions { CancellationToken = cancellationToken }, async (item, token) =>
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<NewsDbContext>();

            var articleUrl = item.Links.FirstOrDefault()?.Uri.ToString();
            if (await dbContext.Articles.AsNoTracking().AnyAsync(a => a.SourceUrl == articleUrl, token))
            {
                _logger.LogInformation($"Skipped addition of duplicate article from URL: {articleUrl}");
                return;
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

            article.PositivityScore = await _sentimentAnalysisService.AnalyzeSentimentAsync(article.Content);

            dbContext.Articles.Add(article);
            await dbContext.SaveChangesAsync(token);
        });

        _logger.LogInformation("Articles initialized with content and images.");
    
       
    }
}
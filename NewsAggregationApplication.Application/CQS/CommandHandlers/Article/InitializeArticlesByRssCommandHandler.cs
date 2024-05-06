using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.Commands;
using NewsAggregationApplication.UI.Interfaces;

namespace NewsAggregationApplication.UI.CommandHandlers.Article;

public class InitializeArticlesByRssCommandHandler : IRequestHandler<InitializeArticlesByRssCommand>
{
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<InitializeArticlesByRssCommandHandler> _logger;
    private readonly IContentScraper _contentScraper;
    private readonly IImageExtractor _imageExtractor;

    public InitializeArticlesByRssCommandHandler(NewsDbContext dbContext,
        ILogger<InitializeArticlesByRssCommandHandler> logger, IContentScraper contentScraper, IImageExtractor imageExtractor)
    {
        _dbContext = dbContext;
        _logger = logger;
        _contentScraper = contentScraper;
        _imageExtractor = imageExtractor;
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

            var sourceUrl = item.Links.FirstOrDefault()?.Uri.Host;
            var source = await _dbContext.Sources.FirstOrDefaultAsync(s => s.Url == sourceUrl, cancellationToken);
            if (source == null)
            {
                source = new Source { Id = Guid.NewGuid(), Url = sourceUrl, Name = sourceUrl };
                _dbContext.Sources.Add(source);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            var article = new Data.Entities.Article
            {
                Id = Guid.NewGuid(),
                Title = item.Title.Text,
                Description = item.Summary.Text,
                PublishedDate = item.PublishDate.UtcDateTime,
                SourceUrl = articleUrl,
                SourceId = source.Id
            };
            _dbContext.Articles.Add(article);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Extract image and scrape content
            var content = await _contentScraper.ScrapeWebPage(articleUrl);
            var imageUrl = _imageExtractor.ExtractImageUrl(item);

            article.Content = content;
            article.UrlToImage = imageUrl;

            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Article initialized with content and image.");
        }

    }
}
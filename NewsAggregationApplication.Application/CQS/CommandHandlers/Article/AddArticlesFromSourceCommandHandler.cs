using System.ServiceModel.Syndication;
using System.Xml;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.Commands.Article;
using NewsAggregationApplication.Data.Entities;

using NewsAggregationApplication.UI.Interfaces;

namespace NewsAggregationApplication.UI.CommandHandlers.Article;

/*
public class AddArticlesFromSourceCommandHandler: IRequestHandler<AddArticlesFromSourceCommand>
{
    /*private readonly NewsDbContext _dbContext;
    private readonly ILogger<AddArticlesFromSourceCommandHandler> _logger;
    private readonly IImageExtractor _imageExtractor;
    private readonly IContentScraper _contentScraper;

    public AddArticlesFromSourceCommandHandler(NewsDbContext dbContext, ILogger<AddArticlesFromSourceCommandHandler> logger, IImageExtractor imageExtractor, IContentScraper contentScraper)
    {#1#
        /*_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _imageExtractor = imageExtractor;
        _contentScraper = contentScraper;
    }
//<Guid>
    public async Task Handle(AddArticlesFromSourceCommand command, CancellationToken cancellationToken)
    {
        foreach (var item in command.RssData)
        {
            var articleUrl = item.Links.FirstOrDefault()?.Uri.ToString();
            if (await _dbContext.Articles.AnyAsync(a => a.SourceUrl == articleUrl, cancellationToken))
            {
                _logger.LogInformation($"Skipped addition of duplicate article from URL: {articleUrl}");
                continue;
            }

            var sourceUrl =
                item.Links.FirstOrDefault()?.Uri
                    .Host; // Assuming 'sourceUrl' is determined by the host of the article URL
            var source = await _dbContext.Sources.FirstOrDefaultAsync(s => s.Url == sourceUrl, cancellationToken);

            if (source == null)
            {
                source = new Source
                {
                    Id = Guid.NewGuid(),
                    Url = sourceUrl,
                    Name = sourceUrl // Assuming name is the same as URL or adjusted accordingly
                };
                _dbContext.Sources.Add(source);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation($"New source added: {sourceUrl}");
            }

            /*var imageUrl = _imageExtractor.ExtractImageUrl(item);
            var content = await _contentScraper.ScrapeWebPage(articleUrl);#2#

            var article = new Data.Entities.Article
            {

                Title = item.Title.Text,
                Description = item.Summary.Text,
                Content = command.Content,//content,
                PublishedDate = item.PublishDate.UtcDateTime,
                SourceUrl = articleUrl,
                SourceId = source.Id,
                UrlToImage = command.ImageUrl//imageUrl
            };

            _dbContext.Articles.Add(article);
            _logger.LogInformation("Added article with URL: {Url}", articleUrl);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

    }#1#
    /*foreach (var rssLink in request.RssData)
    {
        using var reader = XmlReader.Create(rssLink, new XmlReaderSettings { Async = true });
        var feed = SyndicationFeed.Load(reader);
        reader.Close();

        var source = await _dbContext.Sources.FirstOrDefaultAsync(s => s.Url == rssLink, cancellationToken);
        if (source == null)
        {
            source = new Source { Id = Guid.NewGuid(), Url = rssLink, Name = rssLink };
            _dbContext.Sources.Add(source);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        foreach (var item in feed.Items)
        {
            var articleExists = await _dbContext.Articles.AnyAsync(a => a.SourceUrl == item.Links[0].Uri.ToString(), cancellationToken);
            if (articleExists)
                continue;

            var imageUrl = _imageExtractor.ExtractImageUrl(item);
            var content = await _contentScraper.ScrapeWebPage(item.Links[0].Uri.ToString());

            var article = new Data.Entities.Article
            {
                Id = Guid.NewGuid(),
                Title = item.Title.Text,
                Description = item.Summary.Text,
                Content = content,
                PublishedDate = item.PublishDate.UtcDateTime,
                SourceUrl = item.Links[0].Uri.ToString(),
                SourceId = source.Id,
                UrlToImage = imageUrl
            };

            _dbContext.Articles.Add(article);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    return Guid.NewGuid(); // Return a relevant ID or success indicator#1#
    }
    */

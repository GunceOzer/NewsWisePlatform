using MediatR;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.Commands.Article;
using NewsAggregationApplication.UI.Interfaces;

namespace NewsAggregationApplication.UI.CommandHandlers.Article;

public class ExtractImageCommandHandler:IRequestHandler<ExtractImageCommand>
{
    private readonly IImageExtractor _imageExtractor;
    private readonly ILogger<ExtractImageCommandHandler> _logger;
    private readonly NewsDbContext _dbContext;

    public ExtractImageCommandHandler(IImageExtractor imageExtractor, ILogger<ExtractImageCommandHandler> logger, NewsDbContext dbContext)
    {
        _imageExtractor = imageExtractor;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Handle(ExtractImageCommand command, CancellationToken cancellationToken)
    {
        var imageUrl = await _imageExtractor.ExtractImageUrl(command.Item);
        if (string.IsNullOrEmpty(imageUrl))
        {
            _logger.LogWarning("No image URL extracted for Article ID: {ArticleId}", command.ArticleId);
            imageUrl = " "; // Default image placeholder if there is not image
        }
        var article = await _dbContext.Articles.FindAsync(command.ArticleId);
        if (article != null)
        {
            article.UrlToImage = imageUrl;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        _logger.LogInformation("Image extracted and saved for Article ID: {ArticleId}", command.ArticleId);
       
    }
}
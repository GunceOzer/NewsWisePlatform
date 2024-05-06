using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Mappers;
using NewsAggregationApplication.UI.Queries.Article;

namespace NewsAggregationApplication.UI.CQS.QueryHandlers.Article;

public class GetArticlesQueryHandler:IRequestHandler<GetArticlesQuery,IEnumerable<ArticleDto>>
{
    private readonly NewsDbContext _dbContext;
    private readonly ArticleMapper _mapper;
    private readonly ILogger<GetArticlesQueryHandler> _logger;

    public GetArticlesQueryHandler(NewsDbContext dbContext, ArticleMapper mapper, ILogger<GetArticlesQueryHandler> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ArticleDto>> Handle(GetArticlesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all articles with bookmark status.");

        var articles = await _dbContext.Articles
            .Include(a => a.Likes)
            .Select(a => new {
                a.Id,
                a.Title,
                a.Description,
                Content = a.Content ?? "No content available", // Handling NULL content
                a.PublishedDate,
                a.UrlToImage,
                LikesCount = a.Likes.Count,
                IsBookmarked = request.UserId.HasValue && a.Bookmarks.Any(b => b.UserId == request.UserId.Value && b.ArticleId == a.Id) // Checking bookmarks safely
            })
            .ToListAsync(cancellationToken);

        var articleDtos = articles.Select(x => new ArticleDto {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            Content = x.Content,
            PublishedDate = x.PublishedDate,
            UrlToImage = x.UrlToImage,
            LikesCount = x.LikesCount,
            IsBookmarked = x.IsBookmarked
        }).ToList();

        return articleDtos;
        
        
    }
}
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.CQS.Queries.Comment;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Mappers;

namespace NewsAggregationApplication.UI.CQS.QueryHandlers.Comment;

public class GetCommentsByArticleIdAsyncQueryHandler:IRequestHandler<GetCommentsByArticleIdAsyncQuery,List<CommentDto>>
{
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<GetCommentsByArticleIdAsyncQueryHandler> _logger;
    private readonly CommentMapper _mapper;

    public GetCommentsByArticleIdAsyncQueryHandler(NewsDbContext dbContext, CommentMapper mapper, ILogger<GetCommentsByArticleIdAsyncQueryHandler> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<CommentDto>> Handle(GetCommentsByArticleIdAsyncQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var comments = await _dbContext.Comments
                .Where(c => c.ArticleId == request.ArticleId)
                .Include(c => c.User)
                .ToListAsync();

            var commentDtos = comments.Select(c => _mapper.CommentModelToCommentDto(c)).ToList();
            _logger.LogInformation($"Retrieved comments for ArticleID={request.ArticleId}");
            return commentDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to retrieve comments for ArticleID={request.ArticleId}");
            return new List<CommentDto>();
        }
    }
}
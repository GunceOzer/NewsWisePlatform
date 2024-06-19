using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.Commands;
using NewsAggregationApplication.UI.CQS.Commands.Article;
using NewsAggregationApplication.UI.Queries.Article;

namespace NewsAggregationApplication.UI.CommandHandlers.Article;

public class AggregateArticlesCommandHandler:IRequestHandler<AggregateArticlesCommand>
{
    private readonly NewsDbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly ILogger<AggregateArticlesCommandHandler> _logger;

    public AggregateArticlesCommandHandler(NewsDbContext dbContext, IMediator mediator, ILogger<AggregateArticlesCommandHandler> logger)
    {
        _dbContext = dbContext;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(AggregateArticlesCommand request, CancellationToken cancellationToken)
    {
        var sources = await _dbContext.Sources.ToListAsync(cancellationToken);
        foreach (var source in sources)
        {
            try
            {
                var feed = await _mediator.Send(new FetchRssFeedQuery { Url = source.Url }, cancellationToken);
                await _mediator.Send(new InitializeArticlesByRssCommand { RssData = feed.Items, SourceId = source.Id }, cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error processing RSS link from source {source.Name} at {source.Url}");
            }
        }
    }
}
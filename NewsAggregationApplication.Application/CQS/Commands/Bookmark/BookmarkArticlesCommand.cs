using MediatR;
using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.CQS.Commands.Bookmark;

public class BookmarkArticlesCommand:IRequest<bool>
{
    public BookmarkDto BookmarkDto { get; set; }
}
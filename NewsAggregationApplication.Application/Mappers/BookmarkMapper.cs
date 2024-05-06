using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Models;
using Riok.Mapperly.Abstractions;

namespace NewsAggregationApplication.UI.Mappers;

[Mapper]
public partial class BookmarkMapper
{
    //[MapProperty(nameof(BookmarkDto.IsBookmarked), nameof(ArticleModel.IsBookmarked))]
    //public partial BookmarkModel BookmarkDtoToBookmarkModel(BookmarkDto bookmarkDto);
    
    [MapProperty(nameof(BookmarkDto.IsBookmarked), nameof(ArticleModel.IsBookmarked))]
    public partial ArticleModel BookmarkDtoToArticleModel(BookmarkDto bookmarkDto);
    public partial BookmarkDto BookmarkToBookmarkDto(Bookmark bookmark);
}
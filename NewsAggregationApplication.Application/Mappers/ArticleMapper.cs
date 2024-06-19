using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Models;
using Riok.Mapperly.Abstractions;


namespace NewsAggregationApplication.UI.Mappers;

[Mapper]
public partial class ArticleMapper
{ 
   
   public  ArticleModel ArticleDtoToArticleModel(ArticleDto article)
   {
      if (article == null) throw new ArgumentNullException(nameof(article));
      return new ArticleModel
      {
         Id = article.Id,
         Title = article.Title ?? string.Empty,
         Description = article.Description ?? string.Empty,
         Content = article.Content ?? string.Empty,
         PublishedDate = article.PublishedDate,
         LikesCount = article.LikesCount,
         SourceUrl = article.SourceUrl ?? string.Empty,
         UrlToImage = article.UrlToImage ?? string.Empty,
         IsBookmarked = article.IsBookmarked
      };
   }

   public  ArticleDto ArticleToArticleDto(Article article)
   {
      if (article == null) throw new ArgumentNullException(nameof(article));
      return new ArticleDto
      {
         Id = article.Id,
         Title = article.Title ?? string.Empty,
         Description = article.Description ?? string.Empty,
         Content = article.Content ?? string.Empty,
         PublishedDate = article.PublishedDate,
         LikesCount = article.Likes?.Count ?? 0,
         SourceUrl = article.SourceUrl ?? string.Empty,
         UrlToImage = article.UrlToImage ?? string.Empty,
         IsBookmarked = article.Bookmarks?.Any() ?? false
      };
   }
}
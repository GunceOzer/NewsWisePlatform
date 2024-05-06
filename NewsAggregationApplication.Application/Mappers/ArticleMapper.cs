using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Models;
using Riok.Mapperly.Abstractions;


namespace NewsAggregationApplication.UI.Mappers;

[Mapper]
public partial class ArticleMapper
{
   
   public partial ArticleModel ArticleDtoToArticleModel(ArticleDto article);

  
   public partial ArticleDto ArticleToArticleDto(Article article);

}
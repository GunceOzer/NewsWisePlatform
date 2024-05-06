using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.Commands.Article;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Mappers;
using NewsAggregationApplication.UI.Queries.Article;
using NewsAggregationApplication.UI.Services;
using NSubstitute.ReturnsExtensions;

namespace NewsAggregationApplication.Application.Tests;
using Xunit;
using NSubstitute;
public class ArticleServiceTests
{
    private readonly IArticleService _articleServiceMock;
    private readonly IMediator _mediatorMock;
    private readonly ILogger<ArticleService> _mockLogger;
    private readonly ArticleMapper _mockMapper;
    
    public ArticleServiceTests()
    {
        _mediatorMock = Substitute.For<IMediator>();
        _mockMapper = Substitute.For<ArticleMapper>();
        _mockLogger =  Substitute.For<ILogger<ArticleService>>();
        _articleServiceMock = new ArticleService(null, _mockLogger, _mockMapper, null, null, null, _mediatorMock);
    }

   
   

}
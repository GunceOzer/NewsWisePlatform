using System.ServiceModel.Syndication;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.UI.Interfaces;

namespace NewsAggregationApplication.UI.Services;

public class ImageExtractor:IImageExtractor
{
    private readonly ILogger<ImageExtractor> _logger;

    public ImageExtractor(ILogger<ImageExtractor> logger)
    {
        _logger = logger;
    }

    public string ExtractImageUrl(SyndicationItem item)
    {
        /*var media = "http://search.yahoo.com/mrss/";
        var imageUrl = item.ElementExtensions
            .Where(ext => ext.OuterNamespace == media)
            .Select(ext => {
                var element = ext.GetObject<XElement>();
                if (element.Name.LocalName == "content" || element.Name.LocalName == "thumbnail")
                {
                    return element.Attribute("url")?.Value;
                }
                return null;
            })
            .FirstOrDefault(url => !string.IsNullOrEmpty(url));
        return imageUrl;*/
        try
        {
            var media = "http://search.yahoo.com/mrss/";
            var imageUrl = item.ElementExtensions
                .Where(ext => ext.OuterNamespace == media)
                .Select(ext =>
                {
                    var element = ext.GetObject<XElement>();
                    if (element.Name.LocalName == "content" || element.Name.LocalName == "thumbnail")
                    {
                        return element.Attribute("url")?.Value;
                    }
                    return null;
                })
                .FirstOrDefault(url => !string.IsNullOrEmpty(url));

            if (imageUrl != null)
            {
                _logger.LogInformation($"Extracted image URL: {imageUrl}");
                return imageUrl;
            }
            else
            {
                _logger.LogWarning("No image URL found in the syndication item.");
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract image URL from the syndication item.");
            return null;
        }
    }
}
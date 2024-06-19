using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.UI.Interfaces;
using NReadability;

namespace NewsAggregationApplication.UI.Services;

public class ContentScraper:IContentScraper
{
    private readonly ILogger<ContentScraper> _logger;
    private readonly HttpClient _httpClient;


    public ContentScraper(ILogger<ContentScraper> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<string> ScrapeWebPage(string url)
    {

        string content = string.Empty;
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string html = await response.Content.ReadAsStringAsync();
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);

                // Adjusting to the correct node based on the structure shared
                HtmlNode contentNode = document.DocumentNode.SelectSingleNode(
                    "//div[@id='article-body' or @class='entry-content' or @class='g-post-content  js-post-content' or @class='caas-body'] ");

                if (contentNode != null)
                {
                    foreach (HtmlNode paragraph in contentNode.SelectNodes(".//p"))
                    {
                        string paragraphText = paragraph.InnerText.Trim();
                        if (!string.IsNullOrEmpty(paragraphText))
                        {
                            content += paragraphText +
                                       "<br><br>"; // Adding HTML line breaks between paragraphs for readability
                        }
                    }
                }
                else
                {
                    _logger.LogWarning($"No article body found at URL: {url}");
                }
            }
            else
            {
                _logger.LogError($"Failed to retrieve content from {url}. Status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while scraping content from {url}");
        }

        return content;

    }
    
    public string CleanDescription(SyndicationItem item)
    {
        if (item.Summary != null)
        {
            string description = item.Summary.Text;
            // Regular expression to match all <img> tags
            string imgTagPattern = @"<img[^>]*?>";
            // Regular expression to match YouTube <iframe> tags
            string youtubeIframePattern = @"<iframe[^>]+?youtube\.com[^>]*?>.*?</iframe>";
            // Regular expression to remove everything after "Continue reading"
            string continueReadingPattern = @"<div>\s*Continue reading.*";

            // Remove all <img> tags and YouTube iframes from description
            string cleanedDescription = Regex.Replace(description, imgTagPattern, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            cleanedDescription = Regex.Replace(cleanedDescription, youtubeIframePattern, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            // Remove all content following "Continue reading"
            cleanedDescription = Regex.Replace(cleanedDescription, continueReadingPattern, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            
            return cleanedDescription;
        }
        return string.Empty;
        
        
       
    }

}
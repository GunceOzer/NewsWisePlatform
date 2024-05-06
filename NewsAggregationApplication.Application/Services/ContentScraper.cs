using System.Text;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.UI.Interfaces;

namespace NewsAggregationApplication.UI.Services;

public class ContentScraper:IContentScraper
{
    private readonly ILogger<ContentScraper> _logger;
    

    public ContentScraper(ILogger<ContentScraper> logger)
    {
        _logger = logger;
    }

    public async Task<string> ScrapeWebPage(string url)
    {
        /*try
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);

            // XPath is for selecting paragraphs
            var paragraphs = doc.DocumentNode.SelectNodes("//section[@data-component='text-block']//p");

            if (paragraphs != null)
            {
                StringBuilder content = new StringBuilder();
                foreach (var paragraph in paragraphs)
                {
                    // Check if paragraph has text and is not only white spaces
                    if (!string.IsNullOrWhiteSpace(paragraph.InnerText))
                    {
                        content.AppendLine(paragraph.InnerText.Trim());
                    }
                }
                var scrapedContent = content.ToString();
                _logger.LogInformation("Scraped content: {Content}", scrapedContent);
                return scrapedContent;
            }
            else
            {
                _logger.LogWarning("No paragraphs found at {URL}", url);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while scraping the webpage at {URL}", url);
        }

        return string.Empty;
        */
        try
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);
            var paragraphs = doc.DocumentNode.SelectNodes("//section[@data-component='text-block']//p");

            if (paragraphs != null)
            {
                StringBuilder content = new StringBuilder();
                foreach (var paragraph in paragraphs)
                {
                    if (!string.IsNullOrWhiteSpace(paragraph.InnerText))
                    {
                        content.AppendLine(paragraph.InnerText.Trim());
                    }
                }
                _logger.LogInformation($"Successfully scraped content from URL: {url}");
                return content.ToString();
            }
            else
            {
                _logger.LogWarning($"No paragraphs found at URL: {url}");
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while scraping the webpage at URL: {url}");
            return string.Empty;
        }

    }
    /*public async Task<string> ScrapeWebPage(string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to retrieve content from {url}. Status code: {response.StatusCode}");
                return string.Empty;
            }

            var html = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Example: Getting paragraphs from a section that might contain article content.
            // This XPath might need to be adjusted depending on the structure of the HTML page.
            var paragraphs = doc.DocumentNode.SelectNodes("//div[contains(@class, 'article-body')]//p");

            if (paragraphs == null)
            {
                _logger.LogWarning($"No paragraphs found at {url}");
                return string.Empty;
            }

            var contentBuilder = new System.Text.StringBuilder();
            foreach (var paragraph in paragraphs)
            {
                contentBuilder.AppendLine(paragraph.InnerText.Trim());
            }

            return contentBuilder.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while scraping the webpage at {url}");
            return string.Empty;
        }
    }*/
}
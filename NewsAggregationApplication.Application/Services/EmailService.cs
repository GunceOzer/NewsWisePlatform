using System.Net.Http.Headers;
using System.Text;
using Mailgun;
using Microsoft.Extensions.Configuration;
using NewsAggregationApplication.UI.Interfaces;

namespace NewsAggregationApplication.UI.Services;

public class EmailService:IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;


    public EmailService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var apiKey = _configuration["Mailgun:ApiKey"];
        var domain = _configuration["Mailgun:Domain"];
        var fromEmail = _configuration["Mailgun:FromEmail"];

        var requestUri = $"https://api.mailgun.net/v3/{domain}/messages";
        var requestContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("from", fromEmail),
            new KeyValuePair<string, string>("to", email),
            new KeyValuePair<string, string>("subject", subject),
            new KeyValuePair<string, string>("html", message)
        });

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{apiKey}")));

        // Logging the email for debugging 
        Console.WriteLine($"Sending email to: {email}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {message}");
        
        var response = await _httpClient.PostAsync(requestUri, requestContent);
        // Logging the response for debugging
        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Email sent response: {responseContent}");
        
        response.EnsureSuccessStatusCode();
    }
}
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using Mailgun;
using Microsoft.Extensions.Configuration;
using NewsAggregationApplication.UI.Interfaces;

namespace NewsAggregationApplication.UI.Services;

public class EmailService:IEmailService
{
    private readonly IConfiguration _configuration;


    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var smtpSettings = _configuration.GetSection("Smtp");

        var fromAddress = new MailAddress(smtpSettings["FromEmail"], "NewsWise");
        var toAddress = new MailAddress(email);
        string fromPassword = smtpSettings["Password"];

        var smtp = new SmtpClient
        {
            Host = smtpSettings["Host"],
            Port = int.Parse(smtpSettings["Port"]),
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        };

        using (var mailMessage = new MailMessage(fromAddress, toAddress)
               {
                   Subject = subject,
                   Body = message,
                   IsBodyHtml = true
               })
        {
            await smtp.SendMailAsync(mailMessage);
        }
    }
}

   
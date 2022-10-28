using InStockWatch.Models;
using InStockWatch.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;

namespace InStockWatch.Services
{
    public interface INotificationService
    {
        public void SendNotification(Product product);
    }

    public class NotificationService : INotificationService
    {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        private readonly NotificationServiceOptions options;

        public NotificationService(
            IConfiguration configuration,
            ILogger<NotificationService> logger)
        {
            this.logger = logger;
            this.configuration = configuration;
            options = configuration
                .GetSection("NotificationService")
                .Get<NotificationServiceOptions>();
        }

        public void SendNotification(Product product)
        {
            logger.LogInformation(
                @"{0} was called at {1}",
                nameof(NotificationService),
                DateTime.Now.ToString(),
                product);

            SendEmail(product);
        }

        private void SendEmail(Product product)
        {
            var senderEmail = options.SenderEmail;
            var senderPassword = configuration[$"{nameof(NotificationService)}:SenderPassword"];

            var receiverEmail = options.ReceiverEmail;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(
                    senderEmail,
                    senderPassword),
                EnableSsl = true
            };

            var body =
                $"<p>Product is in stock at {DateTime.Now}</p>" +
                $"<p>{product.DisplayName}</p>";

            var subject = "InStockWatch: PRODUCT FOUND!";

            var message = new MailMessage(
                senderEmail,
                receiverEmail,
                subject,
                body)
            {
                IsBodyHtml = true,
                Priority = MailPriority.High
            };

            smtpClient.Send(message);
        }
    }
}

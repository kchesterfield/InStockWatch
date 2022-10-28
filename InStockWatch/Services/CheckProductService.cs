using InStockWatch.Models;
using InStockWatch.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InStockWatch.Services
{
    public interface ICheckProductService : IDisposable
    {
        Task CheckProduct(Product product);
    }

    public class CheckProductService : ICheckProductService
    {
        private readonly ILogger logger;
        private readonly IWebDriver webDriver;
        private readonly CheckProductServiceOptions options;
        private readonly INotificationService notificationService;

        public CheckProductService(
            ILogger<CheckProductService> logger,
            IConfiguration configuration,
            INotificationService notificationService)
        {
            this.logger = logger;
            this.notificationService = notificationService;
            options = configuration
                .GetSection(nameof(CheckProductService))
                .Get<CheckProductServiceOptions>();
            webDriver = GetWebDriver();
        }

        public Task CheckProduct(Product item)
        {
            try
            {
                webDriver
                    .Navigate()
                    .GoToUrl(item.ProductUri);

                var innerText = webDriver
                    .FindElement(By.ClassName(item.AddToCartElement))
                    .Text;

                if (!string.IsNullOrWhiteSpace(innerText)
                    || innerText == item.SoldOutElementText)
                {
                    logger.LogInformation(
                        @"{0}: {1} - {2}",
                        DateTime.Now.ToString(),
                        innerText,
                        item.DisplayName);
                }
                else
                {
                    logger.LogWarning(
                        @"Found unknown text at {0}: {1}",
                        DateTime.Now.ToString(),
                        innerText);

                    notificationService.SendNotification(item);

                    // ToDo: Add product to cart to reserve it
                }
            }
            catch (Exception e)
            {
                logger.LogError(
                    @"Error at {0}: {1}",
                    DateTime.Now.ToString(),
                    e.Message);
            }

            return Task.CompletedTask;
        }

        // ToDo: Move to its own service
        private IWebDriver GetWebDriver()
        {
            // Find any GeckoDriver processes
            var geckoProcesses = Process
                .GetProcesses()
                .Where(x => x.ProcessName.Contains(
                    "geckodriver",
                    StringComparison.CurrentCultureIgnoreCase));

            // Kill any GeckoDriver processes if they are still active
            if (geckoProcesses.Any())
            {
                foreach (var process in geckoProcesses)
                {
                    process.Kill();
                }
            }

            var service = FirefoxDriverService.CreateDefaultService(
                Directory.GetCurrentDirectory() + "\\WebDrivers");

            service.HideCommandPromptWindow = true;

            var firefoxOptions = new FirefoxOptions
            {
                LogLevel = FirefoxDriverLogLevel.Fatal
            };

            if (options.HeadlessMode)
            {
                firefoxOptions.AddArgument("--headless");
            }

            return new FirefoxDriver(
                service,
                firefoxOptions);
        }

        public void Dispose()
        {
            webDriver.Quit();
            webDriver.Dispose();
        }
    }
}

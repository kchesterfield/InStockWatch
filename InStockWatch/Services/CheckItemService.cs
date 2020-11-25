using InStockWatch.Models;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.IO;

namespace InStockWatch.Services
{
    public interface ICheckItemService
    {
        void CheckItem(Item item);
    }

    public class CheckItemService : ICheckItemService
    {
        private readonly ILogger _logger;
        private IWebDriver _webDriver;

        public CheckItemService(
            ILogger<CheckItemService> logger)
        {
            _logger = logger;
        }

        public void CheckItem(Item item)
        {
            _logger.LogDebug(@"{0} has started at {1}", nameof(ProcessingService), DateTime.Now.ToString());

            _webDriver = GetWebDriver();

            _webDriver.Navigate().GoToUrl(item.ItemUri);

            var innerText = _webDriver.FindElement(By.ClassName(item.AddToCartElement)).Text;

            if(string.IsNullOrWhiteSpace(innerText))
            {
                _logger.LogError(@"{0}: {1}", DateTime.Now.ToString(), innerText);
            }
            else if (innerText == item.SoldOutElementText)
            {
                _logger.LogInformation(@"{0}: {1} - {2}", DateTime.Now.ToString(), innerText, item.DisplayName);
            }
            else
            {
                _logger.LogWarning(@"{0} found unknown text at {1}: {2}", nameof(ProcessingService), DateTime.Now.ToString(), innerText);
            }

            _webDriver.Quit();

            _logger.LogDebug(@"{0} has ended at {1}", nameof(ProcessingService), DateTime.Now.ToString());
        }

        private IWebDriver GetWebDriver()
        {
            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService(Directory.GetCurrentDirectory());
            service.HideCommandPromptWindow = true;
            var options = new FirefoxOptions();
            options.AddArgument("--headless");
            options.LogLevel = FirefoxDriverLogLevel.Fatal;
            return new FirefoxDriver(service, options);
        }
    }
}

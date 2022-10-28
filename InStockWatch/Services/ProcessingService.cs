using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InStockWatch.Services
{
    public class ProcessingService : IHostedService
    {
        private readonly IHostApplicationLifetime hostApplicationLifetime;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public ProcessingService(
            IHostApplicationLifetime hostApplicationLifetime,
            IServiceProvider serviceProvider,
            ILogger<ProcessingService> logger)
        {
            this.hostApplicationLifetime = hostApplicationLifetime;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation(
                @"{0} has started at {1}",
                nameof(ProcessingService),
                DateTime.Now.ToString());

            CheckProducts(cancellationToken);

            hostApplicationLifetime.StopApplication();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation(
                @"{0} has stopped at {1}",
                nameof(ProcessingService),
                DateTime.Now.ToString());

            return Task.CompletedTask;
        }

        private Task CheckProducts(CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation(
                    @"{0} creating a new scope and checking products at {1}",
                    nameof(ProcessingService),
                    DateTime.Now.ToString());

                // Creating a new scope everytime ensures that memory leaks
                // are cleaned up, such as the webdriver is a constant issue.
                // This comes at a cost of increased start up time of each loop.
                using var scope = serviceProvider.CreateScope();

                // Essentially turns the transient services into a scoped
                var loadProductsService =
                    scope.ServiceProvider.GetRequiredService<ILoadProductsService>();
                var checkProductService =
                    scope.ServiceProvider.GetRequiredService<ICheckProductService>();

                var products = loadProductsService.LoadProducts();

                if (products == null || products.Count == 0)
                {
                    logger.LogError(
                        @"{0} incurred an error: The list of products to check is null or zero ",
                        nameof(ProcessingService));
                    return Task.CompletedTask;
                }

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    foreach (var product in products)
                    {
                        checkProductService.CheckProduct(product);
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }
                catch (OperationCanceledException e)
                {
                    logger.LogError(
                        @"{0} operation cancelled at {1}: {2}",
                        nameof(ProcessingService),
                        DateTime.Now.ToString(),
                        e.Message);
                }
                catch (Exception e)
                {
                    logger.LogError(
                        @"{0} incurred an error at {1}: {2}",
                        nameof(ProcessingService),
                        DateTime.Now.ToString(),
                        e.Message);
                }
            }

            return Task.CompletedTask;
        }
    }
}

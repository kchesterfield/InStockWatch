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
        private readonly ILogger _logger;

        public ProcessingService(
            IServiceProvider serviceProvider,
            ILogger<ProcessingService> logger)
        {
            ServiceProvider = serviceProvider;
            _logger = logger;
        }

        public IServiceProvider ServiceProvider { get; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug(@"{0} has started at {1}", nameof(ProcessingService), DateTime.Now.ToString());

            await CheckItems(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug(@"{0} has stopped at {1}", nameof(ProcessingService), DateTime.Now.ToString());

            return Task.CompletedTask;
        }

        private Task CheckItems(CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                using var scope = ServiceProvider.CreateScope();

                var loadItemsService =
                    scope.ServiceProvider.GetRequiredService<ILoadItemsService>();

                var items = loadItemsService.LoadItems();

                if (items == null || items.Count == 0)
                {
                    _logger.LogError(@"{0} incurred an error at {1}: The list of items to check is null or zero");
                    return Task.CompletedTask;
                }

                Parallel.ForEach(items, (item) =>
                {
                    var checkItemService =
                    scope.ServiceProvider.GetRequiredService<ICheckItemService>();

                    try
                    {
                        checkItemService.CheckItem(item);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(@"{0} incurred an error at {1}: {2}", nameof(ProcessingService), DateTime.Now.ToString(), e.Message);
                    }
                });
            }
            return Task.CompletedTask;
        }
    }
}

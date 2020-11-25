using InStockWatch.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace InStockWatch
{
    class Program
    {
        static async Task Main()
        {
            IHost host = CreateHostBuilder().Build();

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
            .UseConsoleLifetime(opts =>
            {
                opts.SuppressStatusMessages = true;
            })
            .ConfigureHostConfiguration(configHost =>
            {
                configHost.SetBasePath(Directory.GetCurrentDirectory());
                configHost.AddJsonFile("items.json", optional: false, reloadOnChange: true);
                configHost.AddEnvironmentVariables();
            })
            .ConfigureServices(services =>
            {
                services.AddSingleton<ILoadItemsService, LoadItemsService>();
                services.AddTransient<ICheckItemService, CheckItemService>();
                services.AddHostedService<ProcessingService>();
            });     
    }
}

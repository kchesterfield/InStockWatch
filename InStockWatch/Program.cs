using InStockWatch.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace InStockWatch
{
    public class Program
    {
        public static async Task Main()
        {
            var host = CreateHostBuilder().Build();

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder() => Host
            .CreateDefaultBuilder()
            .UseConsoleLifetime(opts =>
            {
                opts.SuppressStatusMessages = true;
            })
            .ConfigureHostConfiguration(configHost =>
            {
                configHost.SetBasePath(Directory.GetCurrentDirectory());
                configHost.AddJsonFile(
                    "products.json",
                    optional: false,
                    reloadOnChange: true);
                configHost.AddJsonFile(
                    "appsettings.json",
                    optional: false,
                    reloadOnChange: true);
                configHost.AddEnvironmentVariables();
                configHost.AddUserSecrets<Program>();
            })
            .ConfigureServices(services =>
            {
                services.AddTransient<ILoadProductsService, LoadProductsService>();
                services.AddTransient<ICheckProductService, CheckProductService>();
                services.AddTransient<INotificationService, NotificationService>();
                services.AddHostedService<ProcessingService>();
            });
    }
}

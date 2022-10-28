using InStockWatch.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace InStockWatch.Services
{
    public interface ILoadProductsService
    {
        List<Product> LoadProducts();
    }

    public class LoadProductsService : ILoadProductsService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger logger;

        public LoadProductsService(
            IConfiguration configuration,
            ILogger<LoadProductsService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public List<Product> LoadProducts()
        {
            var products = configuration
                .GetSection("Products")
                .Get<List<Product>>();

            if(products == null || products.Count == 0)
            {
                logger.LogError(
                    @"Error at {0}: The list of products returned null or count of zero.",
                    DateTime.Now.ToString());
                return new List<Product>();
            }

            var results = new List<Product>();

            foreach(var product in products)
            {
                if(!product.IsValid())
                {
                    logger.LogError(
                        @"Error at {0}: An invalid product was found within the products.json list file",
                        DateTime.Now.ToString());
                    break;
                }
                results.Add(product);
            }

            return results;
        }
    }
}

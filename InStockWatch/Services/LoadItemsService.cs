using InStockWatch.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace InStockWatch.Services
{
    public interface ILoadItemsService
    {
        List<Item> LoadItems();
    }

    public class LoadItemsService : ILoadItemsService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public LoadItemsService(
            IConfiguration configuration,
            ILogger<LoadItemsService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public List<Item> LoadItems()
        {
            _logger.LogDebug(@"{0} has started at {1}", nameof(LoadItemsService), DateTime.Now.ToString());

            var items = _configuration.GetSection("Items").Get<List<Item>>();

            if(items == null || items.Count == 0)
            {
                _logger.LogError(@"{0} incurred an error at {1}: The list of items returned null or count of zero.", nameof(LoadItemsService), DateTime.Now.ToString());
                return new List<Item>();
            }

            var results = new List<Item>();

            foreach(var item in items)
            {
                if(!item.IsValid())
                {
                    _logger.LogError(@"{0} incurred an error at {1}: An invalid list item was found", nameof(LoadItemsService), DateTime.Now.ToString());
                    break;
                }
                results.Add(item);
            }

            return results;
        }
    }
}

using System;

namespace InStockWatch.Models
{
    public class Item
    {
        public string DisplayName { get; set; }
        public Uri ItemUri { get; set; }
        public string AddToCartElement { get; set; }
        public string SoldOutElementText { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(DisplayName)
                || string.IsNullOrWhiteSpace(AddToCartElement)
                || string.IsNullOrWhiteSpace(SoldOutElementText))
            {
                return false;
            }

            if (ItemUri == null)
            {
                return false;
            }

            return true;
        }
    }
}

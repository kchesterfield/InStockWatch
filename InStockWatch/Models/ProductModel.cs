using System;

namespace InStockWatch.Models
{
    public class Product
    {
        public string DisplayName { get; set; }
        public Uri ProductUri { get; set; }
        public string AddToCartElement { get; set; }
        public string SoldOutElementText { get; set; }

        // ToDo: Update to IValidatableObject
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(DisplayName)
                || string.IsNullOrWhiteSpace(AddToCartElement)
                || string.IsNullOrWhiteSpace(SoldOutElementText))
            {
                return false;
            }

            if (ProductUri == null)
            {
                return false;
            }

            return true;
        }
    }
}

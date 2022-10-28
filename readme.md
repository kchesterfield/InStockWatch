# In Stock Watch

Checks products on a website and reports back if the product is in stock or not.


# Setup

Add products to the `products.json` document. This is a list of all products that you want to check.

Use the Firefox browser to inspect the `Add to Cart` button and get the element name.

You will need to add the email password for the sender account to your user secrets. To do so, run the following command from the InStockWatch project folder:

```
dotnet user-secrets set "NotificationService" "[input-password]"

```


# ToDo

- Use another option instead of Selenium Webdriver
  - Slow to spool up and use, high memory use
  - We may want to keep Selenium Webdriver for checkout purposes
- Create `DateTimeService` and update all `DateTime` to use this instead
  - Easier to unit test and prevent timezone issues


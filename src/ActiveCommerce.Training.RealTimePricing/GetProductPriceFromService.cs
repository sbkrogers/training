﻿using ActiveCommerce.Prices.Pipelines.GetProductTotals;
using Sitecore.Caching;

namespace ActiveCommerce.Training.RealTimePricing
{
    public class GetProductPriceFromService : IGetProductTotalsProcessor
    {
        private static Cache _priceCache;

        static GetProductPriceFromService()
        {
            _priceCache = Cache.GetNamedInstance("acPrices", 1 * 1024 * 1024);
        }

        public void Process(GetProductTotalsArgs args)
        {
            var product = args.Product;
            decimal price = 0;
            if (_priceCache.ContainsKey(product.Code))
            {
                price = (System.Decimal)_priceCache[product.Code];
            }
            else
            {
                var service = new Services.ProductServiceClient();
                price = service.GetPrice(product.Code);
                _priceCache.Add(product.Code, price, 8, System.DateTime.UtcNow.AddMinutes(1));
            }
            args.Totals.PriceExVat = price;
            args.HasPrice = true;
        }
    }
}
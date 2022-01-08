using System;

namespace ProductApi.Responses
{
    public class ProductResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string CurrencyCode { get; set; }
    }
}

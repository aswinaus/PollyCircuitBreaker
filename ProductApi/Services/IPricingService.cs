using System;
using System.Threading.Tasks;
using ProductApi.Models;

namespace ProductApi.Services
{
    public interface IPricingService
    {
        Task<PricingDetails> GetPricingForProductAsync(Guid productId, string currencyCode);
    }
}

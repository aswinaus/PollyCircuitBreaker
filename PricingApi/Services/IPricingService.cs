using System;
using System.Threading.Tasks;
using PricingApi.Models;

namespace PricingApi.Services
{
    public interface IPricingService
    {
        Task<PricingDetails> GetPricingForProductAsync(Guid productId, string currencyCode);
    }
}

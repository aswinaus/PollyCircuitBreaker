using System;
using System.Threading.Tasks;
using ProductApi.Models;

namespace ProductApi.Services
{
    public interface IProductService
    {
        Task<ProductDetails> GetProductDetails(Guid productId);
    }
}

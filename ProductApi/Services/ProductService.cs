using System;
using System.Threading.Tasks;
using ProductApi.Models;

namespace ProductApi.Services
{
    public class ProductService : IProductService
    {
        public Task<ProductDetails> GetProductDetails(Guid productId)
        {
            return Task.FromResult(new ProductDetails
            {
                Id = productId,
                Name = "Apple MacBook Pro"
            });
        }
    }
}

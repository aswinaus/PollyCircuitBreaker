using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Wrap;
using ProductApi.Models;

namespace ProductApi.Services
{
    public class PricingService : IPricingService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private static readonly Random Jitterer = new Random();
        private static readonly AsyncRetryPolicy<HttpResponseMessage> TransientErrorRetryPolicy =
            Policy.HandleResult<HttpResponseMessage>(
                    message => ((int)message.StatusCode) == 429 || (int)message.StatusCode >= 500)
                .WaitAndRetryAsync(2, retryAttempt  =>
                {
                    Console.WriteLine($"Retrying because of transient error. Attempt {retryAttempt}");
                    return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                           + TimeSpan.FromMilliseconds(Jitterer.Next(0, 1000));
                });

        private static readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreakerPolicy =
            Policy.HandleResult<HttpResponseMessage>(message => (int) message.StatusCode == 503)
                // .AdvancedCircuitBreakerAsync(0.5,
                //     TimeSpan.FromMinutes(1),
                //     100,
                //     TimeSpan.FromMinutes(1));
                .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1));

        private readonly AsyncPolicyWrap<HttpResponseMessage> _resilientPolicy =
            CircuitBreakerPolicy.WrapAsync(TransientErrorRetryPolicy);

        public PricingService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<PricingDetails> GetPricingForProductAsync(Guid productId, string currencyCode)
        {
            if (CircuitBreakerPolicy.CircuitState == CircuitState.Open)
            {
                throw new Exception("Service is currently unavailable");
            }

            var httpClient = _httpClientFactory.CreateClient();
            var response = await _resilientPolicy.ExecuteAsync(() =>
                    httpClient.GetAsync($"https://localhost:6003/api/pricing/products/{productId}/currencies/{currencyCode}"));

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Service is currently unavailable");
            }

            var responseText = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PricingDetails>(responseText);
        }
    }
}

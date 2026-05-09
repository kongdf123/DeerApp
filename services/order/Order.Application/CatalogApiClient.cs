using System.Net.Http.Json;

namespace Order.Application
{
    public class CatalogApiClient
    {
        private readonly HttpClient _httpClient;

        public CatalogApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductDto?> GetProduct(long productId)
        {
            return await _httpClient.GetFromJsonAsync<ProductDto>(
                $"/products/{productId}");
        }

    }
}

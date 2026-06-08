using System.Net;
using System.Net.Http.Json;
using MyModularStore.Shared.Exceptions;

namespace MyModularStore.Shared.Contracts.Http;

public class ProductHttpClient(HttpClient httpClient) : IProductContract
{
    public async Task<ProductInfo?> GetInfoAsync(int id)
    {
        var response = await httpClient.GetAsync($"api/products/{id}/info");

        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new RemoteResourceNotFoundException($"Product {id} not found in remote service.");

        if (!response.IsSuccessStatusCode)
            throw new RemoteServiceException(
                $"Products service returned {(int)response.StatusCode} for product {id}.");

        return await response.Content.ReadFromJsonAsync<ProductInfo>();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        var response = await httpClient.GetAsync($"api/products/{id}/exists");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        if (!response.IsSuccessStatusCode)
            throw new RemoteServiceException(
                $"Products service returned {(int)response.StatusCode} checking existence of product {id}.");

        return true;
    }
}

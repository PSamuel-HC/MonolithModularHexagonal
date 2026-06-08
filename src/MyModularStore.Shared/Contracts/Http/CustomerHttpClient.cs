using System.Net;
using System.Net.Http.Json;
using MyModularStore.Shared.Exceptions;

namespace MyModularStore.Shared.Contracts.Http;

public class CustomerHttpClient(HttpClient httpClient) : ICustomerContract
{
    public async Task<CustomerInfo?> GetInfoAsync(int id)
    {
        var response = await httpClient.GetAsync($"api/customers/{id}/info");

        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new RemoteResourceNotFoundException($"Customer {id} not found in remote service.");

        if (!response.IsSuccessStatusCode)
            throw new RemoteServiceException(
                $"Customers service returned {(int)response.StatusCode} for customer {id}.");

        return await response.Content.ReadFromJsonAsync<CustomerInfo>();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        var response = await httpClient.GetAsync($"api/customers/{id}/exists");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        if (!response.IsSuccessStatusCode)
            throw new RemoteServiceException(
                $"Customers service returned {(int)response.StatusCode} checking existence of customer {id}.");

        return true;
    }
}

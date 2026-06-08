namespace MyModularStore.Shared.Contracts
{
    public interface ICustomerContract
    {
        Task<CustomerInfo?> GetInfoAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}

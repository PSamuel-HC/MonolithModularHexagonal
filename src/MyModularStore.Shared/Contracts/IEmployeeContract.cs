namespace MyModularStore.Shared.Contracts
{
    public interface IEmployeeContract
    {
        Task<EmployeeInfo?> GetInfoAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}

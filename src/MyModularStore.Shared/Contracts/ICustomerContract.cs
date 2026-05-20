using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.Shared.Contracts
{
    public interface ICustomerContract
    {
        Task<CustomerInfo?> GetInfoAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}

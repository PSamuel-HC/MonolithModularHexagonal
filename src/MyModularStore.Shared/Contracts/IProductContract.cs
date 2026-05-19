using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.Shared.Contracts
{
    public interface IProductContract
    {
        Task<ProductInfo?> GetInfoAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}

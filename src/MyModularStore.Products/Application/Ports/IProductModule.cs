using MyModularStore.Products.Application.DTOs;

namespace MyModularStore.Products.Application.Ports
{
    public interface IProductModule
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<bool> UpdateAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteAsync(int id);


        //Task AddEmbeddingAsync(int productId, string description, float[] embedding);
        //Task<IEnumerable<VectorSearchResultDto>> SearchSimilarAsync(float[] queryVector, int limit = 5);
    }
}

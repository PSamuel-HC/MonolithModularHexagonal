using Npgsql;
using MyModularStore.Products.Application.Ports;
using MyModularStore.Products.Domain;

namespace MyModularStore.Products.Infrastructure
{
    public class NpgsqlProductRepository(NpgsqlDataSource dataSource) : IProductRepository
    {
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var list = new List<Product>();
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT id, name, price, sku, manufacturer, warranty_months, description
                FROM public.products
                ORDER BY id
                """;
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(MapRow(reader));
            return list;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT id, name, price, sku, manufacturer, warranty_months, description
                FROM public.products
                WHERE id = @id
                """;
            cmd.Parameters.AddWithValue("id", id);
            await using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapRow(reader) : null;
        }

        public async Task AddAsync(Product product)
        {
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO public.products (name, price, sku, manufacturer, warranty_months, description)
                VALUES (@name, @price, @sku, @manufacturer, @warranty_months, @description)
                RETURNING id
                """;
            cmd.Parameters.AddWithValue("name", product.Name);
            cmd.Parameters.AddWithValue("price", product.Price);
            cmd.Parameters.AddWithValue("sku", product.SKU);
            cmd.Parameters.AddWithValue("manufacturer", product.Manufacturer);
            cmd.Parameters.AddWithValue("warranty_months", product.WarrantyMonths);
            cmd.Parameters.AddWithValue("description", product.Description);
            product.Id = (int)(await cmd.ExecuteScalarAsync())!;
        }

        public async Task UpdateAsync(Product product)
        {
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                UPDATE public.products
                SET name             = @name,
                    price            = @price,
                    sku              = @sku,
                    manufacturer     = @manufacturer,
                    warranty_months  = @warranty_months,
                    description      = @description
                WHERE id = @id
                """;
            cmd.Parameters.AddWithValue("id", product.Id);
            cmd.Parameters.AddWithValue("name", product.Name);
            cmd.Parameters.AddWithValue("price", product.Price);
            cmd.Parameters.AddWithValue("sku", product.SKU);
            cmd.Parameters.AddWithValue("manufacturer", product.Manufacturer);
            cmd.Parameters.AddWithValue("warranty_months", product.WarrantyMonths);
            cmd.Parameters.AddWithValue("description", product.Description);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM public.products WHERE id = @id";
            cmd.Parameters.AddWithValue("id", product.Id);
            await cmd.ExecuteNonQueryAsync();
        }

        private static Product MapRow(NpgsqlDataReader r) => new()
        {
            Id              = r.GetInt32(r.GetOrdinal("id")),
            Name            = r.GetString(r.GetOrdinal("name")),
            Price           = r.GetDecimal(r.GetOrdinal("price")),
            SKU             = r.GetString(r.GetOrdinal("sku")),
            Manufacturer    = r.GetString(r.GetOrdinal("manufacturer")),
            WarrantyMonths  = r.GetInt32(r.GetOrdinal("warranty_months")),
            Description     = r.GetString(r.GetOrdinal("description")),
        };
    }
}

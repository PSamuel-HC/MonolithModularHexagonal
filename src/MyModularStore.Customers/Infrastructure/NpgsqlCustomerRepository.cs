using Npgsql;
using MyModularStore.Customers.Application.Ports;
using MyModularStore.Customers.Domain;

namespace MyModularStore.Customers.Infrastructure
{
    internal class NpgsqlCustomerRepository(NpgsqlDataSource dataSource) : ICustomerRepository
    {
        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            var list = new List<Customer>();
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT id, name, email, points_balance, is_premium, last_purchase_date
                FROM public.customers
                WHERE deleted_at IS NULL
                ORDER BY id
                """;
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(MapRow(reader));
            return list;
        }

        public async Task<Customer?> GetOneAsync(int id)
        {
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT id, name, email, points_balance, is_premium, last_purchase_date
                FROM public.customers
                WHERE id = @id AND deleted_at IS NULL
                """;
            cmd.Parameters.AddWithValue("id", id);
            await using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapRow(reader) : null;
        }

        public async Task CreateAsync(Customer customer)
        {
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO public.customers (name, email, points_balance, is_premium, last_purchase_date)
                VALUES (@name, @email, @points_balance, @is_premium, @last_purchase_date)
                RETURNING id
                """;
            cmd.Parameters.AddWithValue("name", customer.FullName);
            cmd.Parameters.AddWithValue("email", customer.Email);
            cmd.Parameters.AddWithValue("points_balance", customer.PointsBalance);
            cmd.Parameters.AddWithValue("is_premium", customer.IsPremium);
            cmd.Parameters.AddWithValue("last_purchase_date",
                (object?)customer.LastPurchaseDate ?? DBNull.Value);
            customer.Id = (int)(await cmd.ExecuteScalarAsync())!;
        }

        public async Task UpdateAsync(Customer customer)
        {
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                UPDATE public.customers
                SET name = @name,
                    email = @email,
                    points_balance = @points_balance,
                    is_premium = @is_premium,
                    last_purchase_date = @last_purchase_date
                WHERE id = @id AND deleted_at IS NULL
                """;
            cmd.Parameters.AddWithValue("id", customer.Id);
            cmd.Parameters.AddWithValue("name", customer.FullName);
            cmd.Parameters.AddWithValue("email", customer.Email);
            cmd.Parameters.AddWithValue("points_balance", customer.PointsBalance);
            cmd.Parameters.AddWithValue("is_premium", customer.IsPremium);
            cmd.Parameters.AddWithValue("last_purchase_date",
                (object?)customer.LastPurchaseDate ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(Customer customer)
        {
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE public.customers SET deleted_at = NOW() WHERE id = @id";
            cmd.Parameters.AddWithValue("id", customer.Id);
            await cmd.ExecuteNonQueryAsync();
        }

        private static Customer MapRow(NpgsqlDataReader r) => new()
        {
            Id               = r.GetInt32(r.GetOrdinal("id")),
            FullName         = r.GetString(r.GetOrdinal("name")),
            Email            = r.GetString(r.GetOrdinal("email")),
            PointsBalance    = r.GetInt32(r.GetOrdinal("points_balance")),
            IsPremium        = r.GetBoolean(r.GetOrdinal("is_premium")),
            LastPurchaseDate = r.IsDBNull(r.GetOrdinal("last_purchase_date"))
                                   ? null
                                   : r.GetDateTime(r.GetOrdinal("last_purchase_date")),
        };
    }
}

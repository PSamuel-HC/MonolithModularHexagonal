using MyModularStore.Orders.Application.Ports;
using MyModularStore.Orders.Domain.Entities;
using MyModularStore.Orders.Domain.Enums;
using Npgsql;

namespace MyModularStore.Orders.Infrastructure
{
    public class NpgsqlOrderRepository(NpgsqlDataSource dataSource) : IOrderRepository
    {
        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default)
        {
            await using var conn = await dataSource.OpenConnectionAsync(ct);
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT id, order_number, customer_id, total_amount, status, created_at
                FROM   public.orders
                ORDER  BY id
                """;

            var orders = new List<Order>();
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
                orders.Add(MapRow(reader));

            return orders;
        }

        public async Task<Order?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var conn = await dataSource.OpenConnectionAsync(ct);
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT id, order_number, customer_id, total_amount, status, created_at
                FROM   public.orders
                WHERE  id = @id
                """;
            cmd.Parameters.AddWithValue("id", id);

            await using var reader = await cmd.ExecuteReaderAsync(ct);
            return await reader.ReadAsync(ct) ? MapRow(reader) : null;
        }

        public async Task AddAsync(Order order, CancellationToken ct = default)
        {
            await using var conn = await dataSource.OpenConnectionAsync(ct);
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO public.orders (customer_id, order_number, total_amount, status)
                VALUES (@customerId, @orderNumber, @totalAmount, @status)
                RETURNING id, created_at
                """;

            cmd.Parameters.AddWithValue("customerId",  order.CustomerId);
            cmd.Parameters.AddWithValue("orderNumber", order.OrderNumber);
            cmd.Parameters.AddWithValue("totalAmount", order.TotalAmount);
            cmd.Parameters.AddWithValue("status",      order.Status.ToString());

            await using var reader = await cmd.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                order.Id        = reader.GetInt32(0);
                order.CreatedAt = reader.GetDateTime(1);
            }
        }

        public async Task UpdateAsync(Order order, CancellationToken ct = default)
        {
            await using var conn = await dataSource.OpenConnectionAsync(ct);
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                UPDATE public.orders
                SET    customer_id  = @customerId,
                       order_number = @orderNumber,
                       total_amount = @totalAmount,
                       status       = @status
                WHERE  id = @id
                """;

            cmd.Parameters.AddWithValue("id",          order.Id);
            cmd.Parameters.AddWithValue("customerId",  order.CustomerId);
            cmd.Parameters.AddWithValue("orderNumber", order.OrderNumber);
            cmd.Parameters.AddWithValue("totalAmount", order.TotalAmount);
            cmd.Parameters.AddWithValue("status",      order.Status.ToString());

            await cmd.ExecuteNonQueryAsync(ct);
        }

        public async Task DeleteAsync(Order order, CancellationToken ct = default)
        {
            await using var conn = await dataSource.OpenConnectionAsync(ct);
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM public.orders WHERE id = @id";
            cmd.Parameters.AddWithValue("id", order.Id);

            await cmd.ExecuteNonQueryAsync(ct);
        }

        private static Order MapRow(NpgsqlDataReader r) => new()
        {
            Id          = r.GetInt32(r.GetOrdinal("id")),
            OrderNumber = r.GetString(r.GetOrdinal("order_number")),
            CustomerId  = r.GetInt32(r.GetOrdinal("customer_id")),
            TotalAmount = r.GetDecimal(r.GetOrdinal("total_amount")),
            Status      = Enum.Parse<OrderStatus>(r.GetString(r.GetOrdinal("status"))),
            CreatedAt   = r.GetDateTime(r.GetOrdinal("created_at")),
        };
    }
}

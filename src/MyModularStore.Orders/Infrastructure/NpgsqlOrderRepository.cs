using MyModularStore.Orders.Application.DTOs;
using MyModularStore.Orders.Application.Ports;
using MyModularStore.Orders.Domain.Entities;
using MyModularStore.Orders.Domain.Enums;
using Npgsql;
using Polly;
using Polly.Registry;

namespace MyModularStore.Orders.Infrastructure
{
    public class NpgsqlOrderRepository(
        ResiliencePipelineProvider<string> pipelines,
        NpgsqlDataSource dataSource) : IOrderRepository
    {

        private readonly ResiliencePipeline _dbResilience = pipelines.GetPipeline("database");

        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default)
        {
            return await _dbResilience.ExecuteAsync(async token =>
            {
                await using var conn = await dataSource.OpenConnectionAsync(token);
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = """
                SELECT id, order_number, customer_id, total_amount, status, created_at
                FROM   public.orders
                ORDER  BY id
                """;

                var orders = new List<Order>();
                await using var reader = await cmd.ExecuteReaderAsync(token);
                while (await reader.ReadAsync(token))
                    orders.Add(MapRow(reader));

                return orders;
            }, ct);
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

        public async Task<IEnumerable<OrderWithCustomerReadDto>> GetAllWithCustomerAsync(CancellationToken ct = default)
        {
            await using var conn = await dataSource.OpenConnectionAsync(ct);
            await using var cmd = conn.CreateCommand();

            cmd.CommandText = """
                SELECT o.id,
                       o.order_number,
                       o.total_amount,
                       o.status,
                       o.created_at,
                       o.customer_id,
                       c.name  AS customer_name,
                       c.email AS customer_email
                FROM   public.orders o
                LEFT  JOIN public.customers c ON c.id = o.customer_id
                ORDER  BY o.id
                """;

            var results = new List<OrderWithCustomerReadDto>();
            await using var reader = await cmd.ExecuteReaderAsync(ct);

            while (await reader.ReadAsync(ct))
            {
                results.Add(new OrderWithCustomerReadDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    OrderNumber = reader.GetString(reader.GetOrdinal("order_number")),
                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("total_amount")),
                    Status = Enum.Parse<OrderStatus>(reader.GetString(reader.GetOrdinal("status"))),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    CustomerId = reader.GetInt32(reader.GetOrdinal("customer_id")),
                    CustomerName = reader.IsDBNull(reader.GetOrdinal("customer_name"))
                                        ? "Unknown"
                                        : reader.GetString(reader.GetOrdinal("customer_name")),
                    CustomerEmail = reader.IsDBNull(reader.GetOrdinal("customer_email"))
                                        ? string.Empty
                                        : reader.GetString(reader.GetOrdinal("customer_email")),
                });
            }

            return results;
        }
    }
}

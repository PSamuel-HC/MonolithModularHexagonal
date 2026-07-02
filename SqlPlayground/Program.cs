using Npgsql;


const string Conn =
    "Host=localhost;Port=5432;Database=store_db;Username=postgres;Password=postgres";


//await Demo_SelectCustomers();
//await Demo_InsertAndReturn();
//await Demo_SelectWithFilter();
await Demo_UpdateOrderStatus(orderId: 2, newStatus: "Shipped");
await Demo_SoftDelete(customerId: 3);
await Demo_HardDeleteCancelledItems();
await CancelOrder(orderId: 4);

static async Task Demo_SelectCustomers()
{
    Console.WriteLine("\n── All Customers ────────────────────────");

    await using var conn = new NpgsqlConnection(Conn);
    await conn.OpenAsync();

    await using var cmd = new NpgsqlCommand(
        "SELECT id, name, email, created_at FROM customers ORDER BY id", conn);

    await using var reader = await cmd.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        var id = reader.GetInt32(reader.GetOrdinal("id"));
        var name = reader.GetString(reader.GetOrdinal("name"));
        var email = reader.GetString(reader.GetOrdinal("email"));
        var createdAt = reader.GetDateTime(reader.GetOrdinal("created_at"));

        Console.WriteLine($"  [{id}] {name,-20} {email,-30} {createdAt:yyyy-MM-dd}");
    }
}


static async Task Demo_InsertAndReturn()
{
    Console.WriteLine("\n── Insert New Customer ──────────────────");

    await using var conn = new NpgsqlConnection(Conn);
    await conn.OpenAsync();

    await using var cmd = new NpgsqlCommand(
        """
        INSERT INTO customers (name, email)
        VALUES (@name, @email)
        RETURNING id
        """, conn);

    cmd.Parameters.AddWithValue("name", "David Lee");
    cmd.Parameters.AddWithValue("email", "david@example.com");

    var newId = await cmd.ExecuteScalarAsync();
    Console.WriteLine($"  Inserted customer with id: {newId}");
}


static async Task Demo_SelectWithFilter()
{
    Console.WriteLine("\n── Pending Orders ───────────────────────");

    await using var conn = new NpgsqlConnection(Conn);
    await conn.OpenAsync();

    await using var cmd = new NpgsqlCommand(
        """
        SELECT id, order_number, total_amount
        FROM   orders
        WHERE  status = @status
        ORDER  BY created_at DESC
        """, conn);

    cmd.Parameters.AddWithValue("status", "Pending");

    await using var reader = await cmd.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        var id = reader.GetInt32(reader.GetOrdinal("id"));
        var number = reader.GetString(reader.GetOrdinal("order_number"));
        var amount = reader.GetDecimal(reader.GetOrdinal("total_amount"));

        Console.WriteLine($"  [{id}] {number}  ${amount:F2}");
    }
}



static async Task Demo_UpdateOrderStatus(int orderId, string newStatus)
{
    Console.WriteLine($"\n── Update Order {orderId} → '{newStatus}' ──────");

    await using var conn = new NpgsqlConnection(Conn);
    await conn.OpenAsync();

    await using var cmd = new NpgsqlCommand(
        """
        UPDATE orders
        SET    status = @status
        WHERE  id = @id
        RETURNING id, order_number, status
        """, conn);
    cmd.Parameters.AddWithValue("status", newStatus);
    cmd.Parameters.AddWithValue("id", orderId);

    await using var reader = await cmd.ExecuteReaderAsync();
    if (await reader.ReadAsync())
    {
        Console.WriteLine(
            $"  Updated: [{reader.GetInt32(0)}] " +
            $"{reader.GetString(1)} → {reader.GetString(2)}");
    }
    else
    {
        Console.WriteLine($"  Order {orderId} not found — no rows updated.");
    }
}

static async Task Demo_SoftDelete(int customerId)
{
    Console.WriteLine($"\n── Soft Delete Customer {customerId} ────────────");

    await using var conn = new NpgsqlConnection(Conn);
    await conn.OpenAsync();

    // Mark as deleted
    await using var deleteCmd = new NpgsqlCommand(
        """
        UPDATE customers
        SET    deleted_at = NOW()
        WHERE  id = @id AND deleted_at IS NULL
        RETURNING id, name, deleted_at
        """, conn);
    deleteCmd.Parameters.AddWithValue("id", customerId);

    await using var reader = await deleteCmd.ExecuteReaderAsync();
    if (await reader.ReadAsync())
    {
        Console.WriteLine(
            $"  Soft-deleted: [{reader.GetInt32(0)}] {reader.GetString(1)} " +
            $"at {reader.GetDateTime(2):yyyy-MM-dd HH:mm:ss}");
    }
    else
    {
        Console.WriteLine($"  Customer {customerId} not found or already deleted.");
    }
}

static async Task Demo_HardDeleteCancelledItems()
{
    Console.WriteLine("\n── Delete Items from Cancelled Orders ───");

    await using var conn = new NpgsqlConnection(Conn);
    await conn.OpenAsync();

    await using var cmd = new NpgsqlCommand(
        """
        DELETE FROM order_items
        WHERE  order_id IN (
            SELECT id FROM orders WHERE status = 'Cancelled'
        )
        """, conn);

    int deleted = await cmd.ExecuteNonQueryAsync();
    Console.WriteLine($"  {deleted} order item(s) removed.");
}
static async Task CancelOrder(int orderId)
{
    Console.WriteLine($"\n Cancel Order {orderId} ────────────────────────");

    await using var conn = new NpgsqlConnection(Conn);
    await conn.OpenAsync();

    await using var updateCmd = new NpgsqlCommand(
        "UPDATE orders SET status = 'Cancelled' WHERE id = @id", conn);
    updateCmd.Parameters.AddWithValue("id", orderId);
    int rowsAffected = await updateCmd.ExecuteNonQueryAsync();

    if (rowsAffected == 0)
    {
        Console.WriteLine($"  Order {orderId} not found.");
        return;
    }

    await using var deleteCmd = new NpgsqlCommand(
        "DELETE FROM order_items WHERE order_id = @id", conn);
    deleteCmd.Parameters.AddWithValue("id", orderId);
    int itemsDeleted = await deleteCmd.ExecuteNonQueryAsync();

    Console.WriteLine($"  Order {orderId} cancelled. Items removed: {itemsDeleted}");
}





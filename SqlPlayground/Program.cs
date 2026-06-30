using Npgsql;


const string Conn =
    "Host=localhost;Port=5432;Database=store_db;Username=postgres;Password=postgres";


await Demo_SelectCustomers();
await Demo_InsertAndReturn();
await Demo_SelectWithFilter();


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


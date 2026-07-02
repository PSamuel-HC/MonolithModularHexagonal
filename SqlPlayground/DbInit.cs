using Npgsql;


namespace SqlPlayground
{
    public class DbInit
    {
        public static async Task InitProceduresAsync(string connectionString)
        {
            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();

            var proceduresPath = Path.Combine(
                                AppContext.BaseDirectory,
                                "Database",
                                "Procedures");

            var files = Directory.GetFiles(proceduresPath, "*.sql")
                                 .OrderBy(Path.GetFileName);

            foreach (var file in files)
            {
                Console.WriteLine($"Applying {Path.GetFileName(file)}");

                var sql = await File.ReadAllTextAsync(file);

                await using var cmd = new NpgsqlCommand(sql, conn);
                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine("Applied");
            }
        
        }
    }
}

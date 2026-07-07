using Npgsql;
using MyModularStore.Employees.Application.Ports;
using MyModularStore.Employees.Domain;

namespace MyModularStore.Employees.Infrastructure
{
    public class NpgsqlEmployeeRepository(NpgsqlDataSource dataSource) : IEmployeeRepository
    {
        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            var list = new List<Employee>();
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT id, first_name, last_name, role, hourly_rate, hire_date
                FROM public.employees
                ORDER BY id
                """;
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(MapRow(reader));
            return list;
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT id, first_name, last_name, role, hourly_rate, hire_date
                FROM public.employees
                WHERE id = @id
                """;
            cmd.Parameters.AddWithValue("id", id);
            await using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapRow(reader) : null;
        }

        public async Task AddAsync(Employee employee)
        {
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO public.employees (first_name, last_name, role, hourly_rate, hire_date)
                VALUES (@first_name, @last_name, @role, @hourly_rate, @hire_date)
                RETURNING id
                """;
            cmd.Parameters.AddWithValue("first_name", employee.FirstName);
            cmd.Parameters.AddWithValue("last_name", employee.LastName);
            cmd.Parameters.AddWithValue("role", employee.Role);
            cmd.Parameters.AddWithValue("hourly_rate", employee.HourlyRate);
            cmd.Parameters.AddWithValue("hire_date", employee.HireDate);
            employee.Id = (int)(await cmd.ExecuteScalarAsync())!;
        }

        public async Task UpdateAsync(Employee employee)
        {
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                UPDATE public.employees
                SET first_name  = @first_name,
                    last_name   = @last_name,
                    role        = @role,
                    hourly_rate = @hourly_rate,
                    hire_date   = @hire_date
                WHERE id = @id
                """;
            cmd.Parameters.AddWithValue("id", employee.Id);
            cmd.Parameters.AddWithValue("first_name", employee.FirstName);
            cmd.Parameters.AddWithValue("last_name", employee.LastName);
            cmd.Parameters.AddWithValue("role", employee.Role);
            cmd.Parameters.AddWithValue("hourly_rate", employee.HourlyRate);
            cmd.Parameters.AddWithValue("hire_date", employee.HireDate);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(Employee employee)
        {
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM public.employees WHERE id = @id";
            cmd.Parameters.AddWithValue("id", employee.Id);
            await cmd.ExecuteNonQueryAsync();
        }

        private static Employee MapRow(NpgsqlDataReader r) => new()
        {
            Id         = r.GetInt32(r.GetOrdinal("id")),
            FirstName  = r.GetString(r.GetOrdinal("first_name")),
            LastName   = r.GetString(r.GetOrdinal("last_name")),
            Role       = r.GetString(r.GetOrdinal("role")),
            HourlyRate = r.GetDecimal(r.GetOrdinal("hourly_rate")),
            HireDate   = r.GetDateTime(r.GetOrdinal("hire_date")),
        };
    }
}

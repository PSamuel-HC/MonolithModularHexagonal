namespace MyModularStore.Employees.Application.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
        public DateTime HireDate { get; set; }
    }
}

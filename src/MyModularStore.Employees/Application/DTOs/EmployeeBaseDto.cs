namespace MyModularStore.Employees.Application.DTOs
{
    public abstract class EmployeeBaseDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
        public string HireDate { get; set; } = string.Empty;
    }
}

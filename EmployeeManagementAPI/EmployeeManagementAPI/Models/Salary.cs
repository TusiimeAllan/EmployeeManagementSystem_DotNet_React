namespace EmployeeManagementAPI.Models
{
    public class Salary
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal Bonus { get; set; }
        public DateTime Date { get; set; }
    }
}

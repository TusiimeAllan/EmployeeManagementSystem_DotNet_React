using EmployeeManagementAPI.Data;
using EmployeeManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace EmployeeManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        [HttpGet("{id}")]
        //[Authorize]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        [HttpPost]
        //[Authorize]
        public async Task<ActionResult<Employee>> PostEmployee([FromBody] Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> PutEmployee(int id, [FromBody] Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        //[Authorize]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("export")]
        //[Authorize]
        public async Task<IActionResult> ExportEmployees()
        {
            var employees = await _context.Employees.ToListAsync();

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Id,FirstName,LastName,Department,Email,Phone");

            foreach (var employee in employees)
            {
                csvBuilder.AppendLine($"{employee.Id},{employee.FirstName},{employee.LastName},{employee.Department},{employee.Email},{employee.Phone}");
            }

            var csvData = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            var result = new FileContentResult(csvData, "text/csv")
            {
                FileDownloadName = "employees.csv"
            };

            return result;
        }
    }
}

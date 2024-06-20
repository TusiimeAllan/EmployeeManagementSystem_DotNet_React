using Microsoft.AspNetCore.Identity;

namespace EmployeeManagementAPI.Data
{
    public class ApplicationRole : IdentityRole
    {
        public override string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }
}

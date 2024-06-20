﻿using Microsoft.AspNetCore.Identity;

namespace EmployeeManagementAPI.Data
{
    public class ApplicationUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Department { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementAPI.Models
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        public required string Department { get; set; }

        [Required]
        [Phone]
        public required string Phone { get; set; }
    }
}

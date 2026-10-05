using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HealthPolicyManagementSystem.ViewModels
{
    public class EditUserViewModel
    {
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Password { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public int? BranchID { get; set; }

        public int? DepartmentID { get; set; }

        public List<SelectListItem> Branches { get; set; } = new();

        public List<SelectListItem> Departments { get; set; } = new();
    }
}
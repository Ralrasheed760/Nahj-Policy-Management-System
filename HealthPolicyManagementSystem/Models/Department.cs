using System.ComponentModel.DataAnnotations;

namespace HealthPolicyManagementSystem.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string DepartmentName { get; set; } = string.Empty;

        [StringLength(255)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int BranchID { get; set; }

        public Branch? Branch { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}

using System.ComponentModel.DataAnnotations;

namespace HealthPolicyManagementSystem.Models
{
    public class Branch
    {
        [Key]
        public int BranchID { get; set; }

        [Required]
        [StringLength(150)]
        public string BranchName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string BranchType { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}
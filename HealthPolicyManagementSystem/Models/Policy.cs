using System.ComponentModel.DataAnnotations;

namespace HealthPolicyManagementSystem.Models
{
    public class Policy
    {
        [Key]
        public int PolicyID { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public int DepartmentID { get; set; }

        [Required]
        [StringLength(255)]
        public string FilePath { get; set; } = string.Empty;

        public DateTime EffectiveDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Version { get; set; } = string.Empty;

        public int UploadedBy { get; set; }

        public DateTime UploadDate { get; set; }

        public int BranchID { get; set; }
        public string? ContentJson { get; set; }
    }
}
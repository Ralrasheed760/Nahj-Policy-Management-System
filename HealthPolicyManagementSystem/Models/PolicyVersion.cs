using System.ComponentModel.DataAnnotations;

namespace HealthPolicyManagementSystem.Models
{
    public class PolicyVersion
    {
        [Key]
        public int VersionID { get; set; }

        public int PolicyID { get; set; }

        [Required]
        [StringLength(20)]
        public string VersionNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string FilePath { get; set; } = string.Empty;

        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

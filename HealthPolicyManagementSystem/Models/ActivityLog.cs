using System.ComponentModel.DataAnnotations;

namespace HealthPolicyManagementSystem.Models
{
    public class ActivityLog
    {
        [Key]
        public int LogID { get; set; }

        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;

        public DateTime ActionDate { get; set; }
    }
}

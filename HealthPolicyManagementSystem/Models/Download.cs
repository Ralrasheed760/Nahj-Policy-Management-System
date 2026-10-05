using System.ComponentModel.DataAnnotations;

namespace HealthPolicyManagementSystem.Models
{
    public class Download
    {
        [Key]
        public int DownloadID { get; set; }

        public int PolicyID { get; set; }

        public int UserID { get; set; }

        public DateTime DownloadDate { get; set; }
    }
}

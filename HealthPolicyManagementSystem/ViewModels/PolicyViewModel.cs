using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HealthPolicyManagementSystem.ViewModels
{
    public class PolicyViewModel
    {
        public int PolicyID { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int DepartmentID { get; set; }

        [Required]
        public int BranchID { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Version { get; set; } = string.Empty;

        public IFormFile? PolicyFile { get; set; }

        public string? ExistingFilePath { get; set; }

        public List<SelectListItem> Departments { get; set; } = new();

        public List<SelectListItem> Branches { get; set; } = new();
    }
}
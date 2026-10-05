using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthPolicyManagementSystem.Pages.Policies
{
    [Authorize(Roles = "Admin,SubAdmin,Employee")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Policy> Policies { get; set; } = new List<Policy>();

        public Dictionary<int, string> Departments { get; set; } = new();

        public List<Department> DepartmentList { get; set; } = new();

        public bool IsAdmin { get; set; }

        public int? UserDepartmentID { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? DepartmentFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SortBy { get; set; }

        public void OnGet()
        {
            IsAdmin = User.IsInRole("Admin");

            if (User.IsInRole("SubAdmin"))
            {
                var departmentClaim = User.FindFirst("DepartmentID");

                if (departmentClaim != null &&
                    int.TryParse(departmentClaim.Value, out int departmentId))
                {
                    UserDepartmentID = departmentId;
                }
            }

            IQueryable<Department> departmentQuery = _context.Departments
                .Where(d => !d.IsDeleted);

            IQueryable<Policy> query = _context.Policies;

            if (!string.IsNullOrWhiteSpace(Search))
            {
                query = query.Where(p =>
                    p.Title.Contains(Search) ||
                    p.Description.Contains(Search));
            }

            if (DepartmentFilter.HasValue)
            {
                query = query.Where(p =>
                    p.DepartmentID == DepartmentFilter.Value);
            }

            switch (SortBy)
            {
                case "Oldest":
                    query = query.OrderBy(p => p.UploadDate);
                    break;

                case "Title":
                    query = query.OrderBy(p => p.Title);
                    break;

                default:
                    query = query.OrderByDescending(p => p.UploadDate);
                    break;
            }

            DepartmentList = departmentQuery
                .OrderBy(d => d.DepartmentName)
                .ToList();

            Policies = query.ToList();

            Departments = DepartmentList.ToDictionary(
                d => d.DepartmentId,
                d => d.DepartmentName);
        }
    }
}

using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthPolicyManagementSystem.Pages.Departments
{
    [Authorize(Roles = "Admin,SubAdmin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Department> Departments { get; set; } = new List<Department>();

        public async Task OnGetAsync()
        {
            var query = _context.Departments
                .Include(d => d.Branch)
                .Where(d => !d.IsDeleted);

            if (User.IsInRole("SubAdmin"))
            {
                int branchId = int.Parse(
                    User.FindFirst("BranchID")!.Value);

                query = query.Where(d => d.BranchID == branchId);
            }

            Departments = await query.ToListAsync();
        }
    }
}

using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HealthPolicyManagementSystem.Pages.Branches
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Branch> Branches { get; set; } = new List<Branch>();

        public async Task OnGetAsync()
        {
            Branches = await _context.Branches.ToListAsync();
        }
    }
}

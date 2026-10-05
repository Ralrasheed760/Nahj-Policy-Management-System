using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HealthPolicyManagementSystem.Pages.Templates
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Template> Templates { get; set; } = new List<Template>();

        public async Task OnGetAsync()
        {
            Templates = await _context.Templates
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}

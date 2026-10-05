using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthPolicyManagementSystem.Pages.Users
{
    [Authorize(Roles = "Admin,SubAdmin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<User> Users { get; set; } = new List<User>();

        public async Task OnGetAsync()
        {
            if (User.IsInRole("Admin"))
            {
                Users = await _context.Users
                    .ToListAsync();
            }
            else
            {
                int branchId = int.Parse(
                    User.FindFirst("BranchID")!.Value);

                Users = await _context.Users
                    .Where(u => u.BranchID == branchId)
                    .ToListAsync();
            }
        }
    }
}

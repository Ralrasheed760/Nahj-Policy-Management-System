using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HealthPolicyManagementSystem.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ActivityLogService _activityLog;

        public LoginModel(
            ApplicationDbContext context,
            ActivityLogService activityLog)
        {
            _context = context;
            _activityLog = activityLog;
        }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToPage("/Admin/Dashboard");
                }

                if (User.IsInRole("SubAdmin"))
                {
                    return RedirectToPage("/SubAdmin/Dashboard");
                }

                if (User.IsInRole("Employee"))
                {
                    return RedirectToPage("/Employee/Dashboard");
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Username == Username &&
                u.Password == Password);

            if (user == null)
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }

            if (!user.IsActive)
            {
                ErrorMessage = "Your account is inactive. Please contact the system administrator.";
                return Page();
            }

            var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
    new Claim(ClaimTypes.Name, user.Username),
    new Claim(ClaimTypes.Role, user.Role),
    new Claim("FullName", user.FullName),
    new Claim("BranchID", user.BranchID?.ToString() ?? ""),
    new Claim("DepartmentID", user.DepartmentID?.ToString() ?? "")
};

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            _activityLog.Log(
                user.UserID,
                "Logged in");

            if (user.Role == "Admin")
            {
                return RedirectToPage("/Admin/Dashboard");
            }

            if (user.Role == "SubAdmin")
            {
                return RedirectToPage("/SubAdmin/Dashboard");
            }

            if (user.Role == "Employee")
            {
                return RedirectToPage("/Employee/Dashboard");
            }

            ErrorMessage = "Invalid user role.";
            return Page();
        }
    }
}
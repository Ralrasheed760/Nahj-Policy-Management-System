using HealthPolicyManagementSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HealthPolicyManagementSystem.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly ActivityLogService _activityLog;

        public LogoutModel(ActivityLogService activityLog)
        {
            _activityLog = activityLog;
        }

        public async Task<IActionResult> OnGet()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null)
            {
                int userId = int.Parse(userIdClaim.Value);

                _activityLog.Log(
                    userId,
                    "Logged out");
            }

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToPage("/Account/Login");
        }
    }
}  

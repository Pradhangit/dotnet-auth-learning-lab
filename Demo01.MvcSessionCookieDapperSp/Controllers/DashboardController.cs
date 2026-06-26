using Demo01.MvcSessionCookieDapperSp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Demo01.MvcSessionCookieDapperSp.Controllers;

public class DashboardController : Controller
{
    private readonly IUserService _userService;

    public DashboardController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _userService.GetUserByIdAsync(userId.Value);

        if (user == null)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> ViewUsers()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Account");
        }

        var users = await _userService.GetReportingUsersAsync(userId.Value);

        return View(users);
    }
}
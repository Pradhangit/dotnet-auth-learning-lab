using Demo01.MvcSessionCookieDapperSp.Services.Interfaces;
using Demo01.MvcSessionCookieDapperSp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Demo01.MvcSessionCookieDapperSp.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        var username = Request.Cookies["RememberedUsername"];

        var model = new LoginViewModel
        {
            Username = username ?? string.Empty,
            RememberMe = !string.IsNullOrWhiteSpace(username)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers.UserAgent.ToString();

        var user = await _userService.LoginAsync(model, ipAddress, userAgent);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        HttpContext.Session.SetInt32("UserId", user.UserId);
        HttpContext.Session.SetString("FullName", user.FullName);
        HttpContext.Session.SetString("Username", user.Username);
        HttpContext.Session.SetInt32("DepartmentId", user.DepartmentId);
        HttpContext.Session.SetString("DepartmentName", user.DepartmentName);
        HttpContext.Session.SetInt32("DesignationId", user.DesignationId);
        HttpContext.Session.SetString("DesignationName", user.DesignationName);
        HttpContext.Session.SetInt32("LevelNo", user.LevelNo);
        HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());
        HttpContext.Session.SetString("AttendanceLogId", user.AttendanceLogId.ToString());

        if (model.RememberMe)
        {
            Response.Cookies.Append(
                "RememberedUsername",
                user.Username,
                new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(7),
                    HttpOnly = true,
                    IsEssential = true
                }
            );
        }
        else
        {
            Response.Cookies.Delete("RememberedUsername");
        }

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }

        if (!IsAdmin())
        {
            return RedirectToAction("AccessDenied", "Home");
        }

        await LoadDepartmentDropdownAsync();

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }

        if (!IsAdmin())
        {
            return RedirectToAction("AccessDenied", "Home");
        }

        if (!ModelState.IsValid)
        {
            await LoadDepartmentDropdownAsync();
            return View(model);
        }

        var result = await _userService.RegisterUserAsync(model);

        if (!result.Success)
        {
            await LoadDepartmentDropdownAsync();
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction("Register", "Account");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        var attendanceLogIdText = HttpContext.Session.GetString("AttendanceLogId");

        if (long.TryParse(attendanceLogIdText, out var attendanceLogId))
        {
            await _userService.LogoutAsync(attendanceLogId);
        }

        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }

    private bool IsLoggedIn()
    {
        return HttpContext.Session.GetInt32("UserId").HasValue;
    }

    private bool IsAdmin()
    {
        return HttpContext.Session.GetString("IsAdmin") == "True";
    }
    [HttpGet]
    public async Task<JsonResult> GetDesignationsByDepartment(int departmentId)
    {
        var designations = await _userService.GetDesignationsByDepartmentAsync(departmentId);

        return Json(designations);
    }

    [HttpGet]
    public async Task<JsonResult> GetReportingAuthorities(int departmentId, int designationId)
    {
        var reportingAuthorities = await _userService.GetReportingAuthoritiesAsync(
            departmentId,
            designationId
        );

        return Json(reportingAuthorities);
    }
    private async Task LoadDepartmentDropdownAsync()
    {
        var departments = await _userService.GetDepartmentsAsync();

        ViewBag.Departments = new SelectList(
            departments,
            "DepartmentId",
            "DepartmentName"
        );
    }
}
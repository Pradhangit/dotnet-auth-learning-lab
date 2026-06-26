using Demo01.MvcSessionCookieDapperSp.DTOs;
using Demo01.MvcSessionCookieDapperSp.ViewModels;

namespace Demo01.MvcSessionCookieDapperSp.Services.Interfaces;

public interface IUserService
{
    Task<OperationResultDto> RegisterUserAsync(RegisterViewModel model);

    Task<LoggedInUserDto?> LoginAsync(
        LoginViewModel model,
        string? loginIpAddress,
        string? userAgent
    );

    Task<DashboardViewModel?> GetUserByIdAsync(int userId);

    Task<List<ReportingUserViewModel>> GetReportingUsersAsync(int loggedInUserId);

    Task<bool> LogoutAsync(long attendanceLogId);
}
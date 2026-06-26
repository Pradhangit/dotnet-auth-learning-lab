using Demo01.MvcSessionCookieDapperSp.DTOs;
using Demo01.MvcSessionCookieDapperSp.ViewModels;

namespace Demo01.MvcSessionCookieDapperSp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> CheckUsernameExistsAsync(string username);

        Task<int> RegisterUserAsync(RegisterViewModel model, string passwordHash);

        Task<LoggedInUserDto?> GetUserByUsernameAsync(string username);

        Task<DashboardViewModel?> GetUserByIdAsync(int userId);

        Task<List<ReportingUserViewModel>> GetReportingUsersAsync(int loggedInUserId);

        Task<long> InsertLoginLogAsync(LoginLogRequestDto request);

        Task<bool> UpdateLogoutLogAsync(LogoutLogRequestDto request);
        Task<List<DepartmentDropdownDto>> GetDepartmentsAsync();

        Task<List<DesignationDropdownDto>> GetDesignationsByDepartmentAsync(int departmentId);

        Task<List<ReportingAuthorityDropdownDto>> GetReportingAuthoritiesAsync(int departmentId, int designationId);
    }
}

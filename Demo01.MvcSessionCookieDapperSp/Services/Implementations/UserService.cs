using Demo01.MvcSessionCookieDapperSp.DTOs;
using Demo01.MvcSessionCookieDapperSp.Repositories.Interfaces;
using Demo01.MvcSessionCookieDapperSp.Services.Interfaces;
using Demo01.MvcSessionCookieDapperSp.ViewModels;

namespace Demo01.MvcSessionCookieDapperSp.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<OperationResultDto> RegisterUserAsync(RegisterViewModel model)
    {
        var usernameExists = await _userRepository.CheckUsernameExistsAsync(model.Username);

        if (usernameExists)
        {
            return new OperationResultDto
            {
                Success = false,
                Message = "Username already exists."
            };
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        var result = await _userRepository.RegisterUserAsync(model, passwordHash);

        if (result == -1)
        {
            return new OperationResultDto
            {
                Success = false,
                Message = "Username already exists."
            };
        }

        if (result > 0)
        {
            return new OperationResultDto
            {
                Success = true,
                Message = "User registered successfully.",
                ResultId = result
            };
        }

        return new OperationResultDto
        {
            Success = false,
            Message = "Something went wrong while registering user."
        };
    }

    public async Task<LoggedInUserDto?> LoginAsync(
        LoginViewModel model,
        string? loginIpAddress,
        string? userAgent
    )
    {
        var user = await _userRepository.GetUserByUsernameAsync(model.Username);

        if (user == null)
        {
            await _userRepository.InsertLoginLogAsync(new LoginLogRequestDto
            {
                UserId = null,
                Username = model.Username,
                LoginStatus = "Failed",
                FailureReason = "Invalid username",
                LoginIpAddress = loginIpAddress,
                UserAgent = userAgent
            });

            return null;
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            await _userRepository.InsertLoginLogAsync(new LoginLogRequestDto
            {
                UserId = user.UserId,
                Username = user.Username,
                LoginStatus = "Failed",
                FailureReason = "Invalid password",
                LoginIpAddress = loginIpAddress,
                UserAgent = userAgent
            });

            return null;
        }

        var attendanceLogId = await _userRepository.InsertLoginLogAsync(new LoginLogRequestDto
        {
            UserId = user.UserId,
            Username = user.Username,
            LoginStatus = "Success",
            FailureReason = null,
            LoginIpAddress = loginIpAddress,
            UserAgent = userAgent
        });

        user.AttendanceLogId = attendanceLogId;

        return user;
    }

    public async Task<DashboardViewModel?> GetUserByIdAsync(int userId)
    {
        return await _userRepository.GetUserByIdAsync(userId);
    }

    public async Task<List<ReportingUserViewModel>> GetReportingUsersAsync(int loggedInUserId)
    {
        return await _userRepository.GetReportingUsersAsync(loggedInUserId);
    }

    public async Task<bool> LogoutAsync(long attendanceLogId)
    {
        return await _userRepository.UpdateLogoutLogAsync(new LogoutLogRequestDto
        {
            AttendanceLogId = attendanceLogId
        });
    }

    public async Task<List<DepartmentDropdownDto>> GetDepartmentsAsync()
    {
        return await _userRepository.GetDepartmentsAsync();
    }

    public async Task<List<DesignationDropdownDto>> GetDesignationsByDepartmentAsync(int departmentId)
    {
        return await _userRepository.GetDesignationsByDepartmentAsync(departmentId);
    }

    public async Task<List<ReportingAuthorityDropdownDto>> GetReportingAuthoritiesAsync(int departmentId, int designationId
    )
    {
        return await _userRepository.GetReportingAuthoritiesAsync(departmentId, designationId);
    }
}
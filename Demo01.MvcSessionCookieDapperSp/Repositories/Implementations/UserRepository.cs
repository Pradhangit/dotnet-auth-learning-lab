using System.Data;
using Dapper;
using Demo01.MvcSessionCookieDapperSp.DTOs;
using Demo01.MvcSessionCookieDapperSp.Repositories.Interfaces;
using Demo01.MvcSessionCookieDapperSp.ViewModels;
using Microsoft.Data.SqlClient;

namespace Demo01.MvcSessionCookieDapperSp.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly IConfiguration _configuration;

    public UserRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private IDbConnection CreateConnection()
    {
        var connectionString = _configuration.GetConnectionString("Conn");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'Conn' is missing in appsettings.json.");
        }

        return new SqlConnection(connectionString);
    }

    public async Task<bool> CheckUsernameExistsAsync(string username)
    {
        using var connection = CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@Action", "CHECK_USERNAME");
        parameters.Add("@Username", username);

        var result = await connection.QueryFirstOrDefaultAsync<int>(
            "sp_UserManagement5361",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result == 1;
    }

    public async Task<int> RegisterUserAsync(RegisterViewModel model, string passwordHash)
    {
        using var connection = CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@Action", "REGISTER_USER");
        parameters.Add("@FullName", model.FullName);
        parameters.Add("@Username", model.Username);
        parameters.Add("@PasswordHash", passwordHash);
        parameters.Add("@DepartmentId", model.DepartmentId);
        parameters.Add("@DesignationId", model.DesignationId);
        parameters.Add("@ReportingAuthorityId", model.ReportingAuthorityId);
        parameters.Add("@Email", model.Email);
        parameters.Add("@PhoneNumber", model.PhoneNumber);

        var result = await connection.QueryFirstOrDefaultAsync<int>(
            "sp_UserManagement5361",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public async Task<LoggedInUserDto?> GetUserByUsernameAsync(string username)
    {
        using var connection = CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@Action", "LOGIN");
        parameters.Add("@Username", username);

        var user = await connection.QueryFirstOrDefaultAsync<LoggedInUserDto>(
            "sp_UserManagement5361",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return user;
    }

    public async Task<DashboardViewModel?> GetUserByIdAsync(int userId)
    {
        using var connection = CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@Action", "GET_USER_BY_ID");
        parameters.Add("@UserId", userId);

        var user = await connection.QueryFirstOrDefaultAsync<DashboardViewModel>(
            "sp_UserManagement5361",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return user;
    }

    public async Task<List<ReportingUserViewModel>> GetReportingUsersAsync(int loggedInUserId, DateTime attendanceDate)
    {
        using var connection = CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@Action", "GET_REPORTING_USERS");
        parameters.Add("@UserId", loggedInUserId);
        parameters.Add("@AttendanceDate", attendanceDate.Date);

        var users = await connection.QueryAsync<ReportingUserViewModel>(
            "sp_UserManagement5361",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return users.ToList();
    }

    public async Task<long> InsertLoginLogAsync(LoginLogRequestDto request)
    {
        using var connection = CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@Action", "INSERT_LOGIN_LOG");
        parameters.Add("@UserId", request.UserId);
        parameters.Add("@Username", request.Username);
        parameters.Add("@LoginStatus", request.LoginStatus);
        parameters.Add("@FailureReason", request.FailureReason);
        parameters.Add("@LoginIpAddress", request.LoginIpAddress);
        parameters.Add("@UserAgent", request.UserAgent);

        var attendanceLogId = await connection.QueryFirstOrDefaultAsync<long>(
            "sp_UserManagement5361",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return attendanceLogId;
    }

    public async Task<bool> UpdateLogoutLogAsync(LogoutLogRequestDto request)
    {
        using var connection = CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@Action", "UPDATE_LOGOUT_LOG");
        parameters.Add("@AttendanceLogId", request.AttendanceLogId);

        var result = await connection.QueryFirstOrDefaultAsync<int>(
            "sp_UserManagement5361",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result == 1;
    }
    public async Task<List<DepartmentDropdownDto>> GetDepartmentsAsync()
    {
        using var connection = CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@Action", "GET_DEPARTMENTS");

        var departments = await connection.QueryAsync<DepartmentDropdownDto>(
            "sp_UserManagement5361",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return departments.ToList();
    }

    public async Task<List<DesignationDropdownDto>> GetDesignationsByDepartmentAsync(int departmentId)
    {
        using var connection = CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@Action", "GET_DESIGNATIONS_BY_DEPARTMENT");
        parameters.Add("@DepartmentId", departmentId);

        var designations = await connection.QueryAsync<DesignationDropdownDto>(
            "sp_UserManagement5361",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return designations.ToList();
    }

    public async Task<List<ReportingAuthorityDropdownDto>> GetReportingAuthoritiesAsync(
        int departmentId,
        int designationId
    )
    {
        using var connection = CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@Action", "GET_REPORTING_AUTHORITIES");
        parameters.Add("@DepartmentId", departmentId);
        parameters.Add("@DesignationId", designationId);

        var reportingAuthorities = await connection.QueryAsync<ReportingAuthorityDropdownDto>(
            "sp_UserManagement5361",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return reportingAuthorities.ToList();
    }
}
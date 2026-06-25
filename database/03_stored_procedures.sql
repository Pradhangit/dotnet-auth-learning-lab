sp_UserManagementUSE DotNetAuthLearningLabDb;
GO

-- =====================================================
-- Stored Procedure: sp_UserManagement
-- Purpose:
-- This is one action-based stored procedure used for
-- user login, user profile, team view, login log, logout log.
-- =====================================================

CREATE OR ALTER PROCEDURE sp_UserManagement
(
    @Action NVARCHAR(50),

    @UserId INT = NULL,
    @Username NVARCHAR(50) = NULL,
    @PasswordHash NVARCHAR(500) = NULL,

    @FullName NVARCHAR(150) = NULL,
    @DepartmentId INT = NULL,
    @DesignationId INT = NULL,
    @ReportingAuthorityId INT = NULL,
    @Email NVARCHAR(150) = NULL,
    @PhoneNumber NVARCHAR(20) = NULL,

    @AttendanceLogId BIGINT = NULL,
    @LoginIpAddress NVARCHAR(50) = NULL,
    @LogoutIpAddress NVARCHAR(50) = NULL,
    @UserAgent NVARCHAR(500) = NULL,
    @LoginStatus NVARCHAR(20) = NULL,
    @FailureReason NVARCHAR(300) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    -- =====================================================
    -- Action: CHECK_USERNAME
    -- Purpose: Check whether username already exists
    -- =====================================================
    IF (@Action = 'CHECK_USERNAME')
    BEGIN
        SELECT 
            CASE 
                WHEN EXISTS 
                (
                    SELECT 1 
                    FROM Users 
                    WHERE Username = @Username
                )
                THEN 1 
                ELSE 0 
            END AS IsExists;

        RETURN;
    END

    -- =====================================================
    -- Action: REGISTER_USER
    -- Purpose: Register a new user
    -- =====================================================
    IF (@Action = 'REGISTER_USER')
    BEGIN
        IF EXISTS (SELECT 1 FROM Users WHERE Username = @Username)
        BEGIN
            SELECT -1 AS Result;
            RETURN;
        END

        INSERT INTO Users
        (
            DepartmentId,
            DesignationId,
            ReportingAuthorityId,
            FullName,
            Username,
            PasswordHash,
            Email,
            PhoneNumber
        )
        VALUES
        (
            @DepartmentId,
            @DesignationId,
            @ReportingAuthorityId,
            @FullName,
            @Username,
            @PasswordHash,
            @Email,
            @PhoneNumber
        );

        SELECT SCOPE_IDENTITY() AS Result;
        RETURN;
    END

    -- =====================================================
    -- Action: LOGIN
    -- Purpose: Get user by username for login
    -- Password will be verified in C# using PasswordHash
    -- =====================================================
    IF (@Action = 'LOGIN')
    BEGIN
        SELECT
            U.UserId,
            U.FullName,
            U.Username,
            U.PasswordHash,
            U.Email,
            U.PhoneNumber,
            U.DepartmentId,
            D.DepartmentName,
            U.DesignationId,
            DG.DesignationName,
            DG.LevelNo,
            U.ReportingAuthorityId,
            U.IsActive
        FROM Users U
        INNER JOIN Departments D 
            ON U.DepartmentId = D.DepartmentId
        INNER JOIN Designations DG 
            ON U.DesignationId = DG.DesignationId
        WHERE U.Username = @Username
          AND U.IsActive = 1;

        RETURN;
    END

    -- =====================================================
    -- Action: GET_USER_BY_ID
    -- Purpose: Get logged-in user's profile details
    -- =====================================================
    IF (@Action = 'GET_USER_BY_ID')
    BEGIN
        SELECT
            U.UserId,
            U.FullName,
            U.Username,
            U.Email,
            U.PhoneNumber,
            U.DepartmentId,
            D.DepartmentName,
            U.DesignationId,
            DG.DesignationName,
            DG.LevelNo,
            U.ReportingAuthorityId,
            RA.FullName AS ReportingAuthorityName
        FROM Users U
        INNER JOIN Departments D 
            ON U.DepartmentId = D.DepartmentId
        INNER JOIN Designations DG 
            ON U.DesignationId = DG.DesignationId
        LEFT JOIN Users RA 
            ON U.ReportingAuthorityId = RA.UserId
        WHERE U.UserId = @UserId
          AND U.IsActive = 1;

        RETURN;
    END

    -- =====================================================
    -- Action: GET_REPORTING_USERS
    -- Purpose: Get users who directly report to logged-in user
    -- Rule: Users.ReportingAuthorityId = LoggedInUserId
    -- =====================================================
    IF (@Action = 'GET_REPORTING_USERS')
    BEGIN
        SELECT
            U.UserId,
            U.FullName,
            U.Username,
            U.Email,
            U.PhoneNumber,
            D.DepartmentName,
            DG.DesignationName,
            DG.LevelNo,

            L.LoginTime,
            L.LogoutTime,
            L.TotalDurationMinutes,
            L.LunchDeductionMinutes,
            L.NetDurationMinutes
        FROM Users U
        INNER JOIN Departments D 
            ON U.DepartmentId = D.DepartmentId
        INNER JOIN Designations DG 
            ON U.DesignationId = DG.DesignationId
        OUTER APPLY
        (
            SELECT TOP 1
                AttendanceLogId,
                LoginTime,
                LogoutTime,
                TotalDurationMinutes,
                LunchDeductionMinutes,
                NetDurationMinutes
            FROM UserAttendanceLogs
            WHERE UserId = U.UserId
            ORDER BY AttendanceLogId DESC
        ) L
        WHERE U.ReportingAuthorityId = @UserId
          AND U.IsActive = 1
        ORDER BY DG.LevelNo, U.FullName;

        RETURN;
    END

    -- =====================================================
    -- Action: INSERT_LOGIN_LOG
    -- Purpose: Insert login time when user logs in
    -- =====================================================
    IF (@Action = 'INSERT_LOGIN_LOG')
    BEGIN
        INSERT INTO UserAttendanceLogs
        (
            UserId,
            Username,
            LoginTime,
            LoginIpAddress,
            UserAgent,
            LoginStatus,
            FailureReason
        )
        VALUES
        (
            @UserId,
            @Username,
            GETDATE(),
            @LoginIpAddress,
            @UserAgent,
            @LoginStatus,
            @FailureReason
        );

        SELECT SCOPE_IDENTITY() AS AttendanceLogId;
        RETURN;
    END

    -- =====================================================
    -- Action: UPDATE_LOGOUT_LOG
    -- Purpose: Update logout time and calculate duration
    -- =====================================================
    IF (@Action = 'UPDATE_LOGOUT_LOG')
    BEGIN
        UPDATE UserAttendanceLogs
        SET
            LogoutTime = GETDATE(),
            LogoutIpAddress = @LogoutIpAddress,
            TotalDurationMinutes = DATEDIFF(MINUTE, LoginTime, GETDATE()),
            NetDurationMinutes = 
                CASE 
                    WHEN DATEDIFF(MINUTE, LoginTime, GETDATE()) > LunchDeductionMinutes
                    THEN DATEDIFF(MINUTE, LoginTime, GETDATE()) - LunchDeductionMinutes
                    ELSE DATEDIFF(MINUTE, LoginTime, GETDATE())
                END
        WHERE AttendanceLogId = @AttendanceLogId;

        SELECT 1 AS Result;
        RETURN;
    END

    -- =====================================================
    -- Invalid Action
    -- =====================================================
    SELECT -999 AS Result;
END;
GO
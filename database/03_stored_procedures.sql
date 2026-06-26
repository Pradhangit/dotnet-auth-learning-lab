USE DotNetAuthLearningLabDb;
GO

-- =====================================================
-- Stored Procedure: sp_UserManagement
-- Purpose:
-- One action-based stored procedure for:
-- 1. Username check
-- 2. User registration
-- 3. Login user fetch
-- 4. User profile fetch
-- 5. Reporting users fetch
-- 6. Insert login log
-- 7. Update logout log
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
    -- Return: 1 if exists, 0 if not exists
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
    END;

    -- =====================================================
    -- Action: REGISTER_USER
    -- Purpose: Register a new user
    -- Return:
    -- -1 = Username already exists
    -- New UserId = Registration successful
    -- =====================================================
    IF (@Action = 'REGISTER_USER')
    BEGIN
        IF EXISTS
        (
            SELECT 1
            FROM Users
            WHERE Username = @Username
        )
        BEGIN
            SELECT -1 AS Result;
            RETURN;
        END;

        INSERT INTO Users
        (
            DepartmentId,
            DesignationId,
            ReportingAuthorityId,
            FullName,
            Username,
            PasswordHash,
            Email,
            PhoneNumber,
            IsActive,
            CreatedOn
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
            @PhoneNumber,
            1,
            GETDATE()
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Result;
        RETURN;
    END;

    -- =====================================================
    -- Action: LOGIN
    -- Purpose:
    -- Get user by username for login.
    -- Password verification will happen in C# using BCrypt.
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
            D.DepartmentCode,
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
          AND U.IsActive = 1
          AND D.IsActive = 1
          AND DG.IsActive = 1;

        RETURN;
    END;

    -- =====================================================
    -- Action: GET_USER_BY_ID
    -- Purpose:
    -- Get logged-in user's profile/dashboard details.
    -- PasswordHash is not returned here.
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
            D.DepartmentCode,
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
          AND U.IsActive = 1
          AND D.IsActive = 1
          AND DG.IsActive = 1;

        RETURN;
    END;

    -- =====================================================
    -- Action: GET_REPORTING_USERS
    -- Purpose:
    -- Get users directly working under logged-in user.
    -- Rule:
    -- Users.ReportingAuthorityId = LoggedInUserId
    --
    -- Example:
    -- Level 2 sees direct Level 1 users.
    -- Level 3 sees direct Level 2 users.
    -- Level 4 sees direct Level 3 users.
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
              AND LoginStatus = 'Success'
            ORDER BY AttendanceLogId DESC
        ) L
        WHERE U.ReportingAuthorityId = @UserId
          AND U.IsActive = 1
          AND D.IsActive = 1
          AND DG.IsActive = 1
        ORDER BY DG.LevelNo, U.FullName;

        RETURN;
    END;

    -- =====================================================
    -- Action: INSERT_LOGIN_LOG
    -- Purpose:
    -- Insert login attempt.
    -- For successful login, UserId will have value.
    -- For failed login, UserId can be NULL.
    --
    -- Return: AttendanceLogId
    -- =====================================================
    IF (@Action = 'INSERT_LOGIN_LOG')
    BEGIN
        INSERT INTO UserAttendanceLogs
        (
            UserId,
            Username,
            LoginTime,
            LogoutTime,
            TotalDurationMinutes,
            LunchDeductionMinutes,
            NetDurationMinutes,
            LoginIpAddress,
            LogoutIpAddress,
            UserAgent,
            LoginStatus,
            FailureReason,
            CreatedOn
        )
        VALUES
        (
            @UserId,
            @Username,
            GETDATE(),
            NULL,
            NULL,
            30,
            NULL,
            @LoginIpAddress,
            NULL,
            @UserAgent,
            @LoginStatus,
            @FailureReason,
            GETDATE()
        );

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS AttendanceLogId;
        RETURN;
    END;

    -- =====================================================
    -- Action: UPDATE_LOGOUT_LOG
    -- Purpose:
    -- Update logout time and calculate total/net duration.
    --
    -- TotalDurationMinutes = LogoutTime - LoginTime
    -- NetDurationMinutes = TotalDurationMinutes - 30 minutes lunch
    --
    -- Logout IP is not used for now.
    -- =====================================================
    IF (@Action = 'UPDATE_LOGOUT_LOG')
    BEGIN
        UPDATE UserAttendanceLogs
        SET
            LogoutTime = GETDATE(),

            TotalDurationMinutes =
                DATEDIFF(MINUTE, LoginTime, GETDATE()),

            NetDurationMinutes =
                CASE
                    WHEN DATEDIFF(MINUTE, LoginTime, GETDATE()) > LunchDeductionMinutes
                    THEN DATEDIFF(MINUTE, LoginTime, GETDATE()) - LunchDeductionMinutes
                    ELSE DATEDIFF(MINUTE, LoginTime, GETDATE())
                END
        WHERE AttendanceLogId = @AttendanceLogId
          AND LogoutTime IS NULL;

        SELECT
            CASE
                WHEN @@ROWCOUNT > 0 THEN 1
                ELSE 0
            END AS Result;

        RETURN;
    END;

    -- =====================================================
    -- Invalid Action
    -- =====================================================
    SELECT -999 AS Result;
END;
GO
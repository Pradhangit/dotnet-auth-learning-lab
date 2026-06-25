CREATE DATABASE DotNetAuthLearningLabDb;
GO

USE DotNetAuthLearningLabDb;
GO

-- =====================================================
-- 1. Departments
-- =====================================================
CREATE TABLE Departments
(
    DepartmentId INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentName NVARCHAR(100) NOT NULL,
    DepartmentCode NVARCHAR(20) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedOn DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- =====================================================
-- 2. Designations
-- =====================================================
CREATE TABLE Designations
(
    DesignationId INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentId INT NOT NULL,
    DesignationName NVARCHAR(100) NOT NULL,
    LevelNo INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedOn DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Designations_Departments
        FOREIGN KEY (DepartmentId)
        REFERENCES Departments(DepartmentId)
);
GO

-- =====================================================
-- 3. Users
-- =====================================================
CREATE TABLE Users
(
    UserId INT IDENTITY(1,1) PRIMARY KEY,

    DepartmentId INT NOT NULL,
    DesignationId INT NOT NULL,

    -- Immediate senior / reporting authority
    ReportingAuthorityId INT NULL,

    FullName NVARCHAR(150) NOT NULL,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,

    Email NVARCHAR(150) NULL,
    PhoneNumber NVARCHAR(20) NULL,

    IsActive BIT NOT NULL DEFAULT 1,
    CreatedOn DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedOn DATETIME NULL,

    CONSTRAINT FK_Users_Departments
        FOREIGN KEY (DepartmentId)
        REFERENCES Departments(DepartmentId),

    CONSTRAINT FK_Users_Designations
        FOREIGN KEY (DesignationId)
        REFERENCES Designations(DesignationId),

    CONSTRAINT FK_Users_ReportingAuthority
        FOREIGN KEY (ReportingAuthorityId)
        REFERENCES Users(UserId)
);
GO

-- =====================================================
-- 4. UserAttendanceLogs
-- =====================================================
CREATE TABLE UserAttendanceLogs
(
    AttendanceLogId BIGINT IDENTITY(1,1) PRIMARY KEY,

    UserId INT NULL,
    Username NVARCHAR(50) NULL,

    LoginTime DATETIME NULL,
    LogoutTime DATETIME NULL,

    TotalDurationMinutes INT NULL,
    LunchDeductionMinutes INT NOT NULL DEFAULT 30,
    NetDurationMinutes INT NULL,

    LoginIpAddress NVARCHAR(50) NULL,
    LogoutIpAddress NVARCHAR(50) NULL,
    UserAgent NVARCHAR(500) NULL,

    LoginStatus NVARCHAR(20) NOT NULL,
    FailureReason NVARCHAR(300) NULL,

    CreatedOn DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_UserAttendanceLogs_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(UserId)
);
GO
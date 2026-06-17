# Demo 01 - .NET Core MVC + Session + Cookie + Dapper + Stored Procedure

## Project Type

This is a .NET Core MVC project without API.

## Technologies

- .NET Core MVC
- SQL Server
- Dapper
- One action-based stored procedure
- Session
- Cookie
- Git and GitHub

## Not Included

- No API
- No JWT
- No EF Core
- No Angular

## Main Features

- User registration
- Username-password login
- User logout
- Dashboard after login
- Session-based authentication
- Remember-me cookie
- Role-based menu

## Database Tables

1. Roles
2. Users
3. UserRoles
4. Menus
5. RoleMenus
6. UserLoginLogs

## Stored Procedure Strategy

This project uses one action-based stored procedure for user, role, menu, and login related database operations.

Main stored procedure:

- sp_UserManagement

## Stored Procedure Actions

| Action | Purpose |
|---|---|
| RU | Register User |
| GUBU | Get User By Username |
| GUBI | Get User By Id |
| AR | Assign Role To User |
| GR | Get User Roles |
| GM | Get Menus By UserId |
| ILL | Insert Login Log |
| CU | Check Username Exists |

## Authentication Flow

1. User registers with full name, username, and password.
2. Password is hashed in C# before saving.
3. System checks whether username already exists.
4. User is assigned default role: User.
5. User logs in with username and password.
6. System gets user by username using sp_UserManagement with action GUBU.
7. System validates password hash in C#.
8. If login is successful, user details are stored in Session.
9. If Remember Me is checked, username is stored in Cookie.
10. User is redirected to Dashboard.
11. Logout clears Session and Cookie.

## Session Values

- UserId
- FullName
- Username
- RoleName

## Cookie Values

- RememberedUsername

## Default Roles

- Admin
- Manager
- User

## Default Menus

- Dashboard
- My Profile
- Users
- Reports
- Logout
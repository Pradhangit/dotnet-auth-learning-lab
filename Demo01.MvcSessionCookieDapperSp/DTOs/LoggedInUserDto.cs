namespace Demo01.MvcSessionCookieDapperSp.DTOs
{
    public class LoggedInUserDto
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public int DesignationId { get; set; }

        public string DesignationName { get; set; } = string.Empty;

        public int LevelNo { get; set; }

        public int? ReportingAuthorityId { get; set; }

        public bool IsActive { get; set; }
    }
}

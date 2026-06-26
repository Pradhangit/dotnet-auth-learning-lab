namespace Demo01.MvcSessionCookieDapperSp.Models
{
    public class User
    {
        public int UserId { get; set; }

        public int DepartmentId { get; set; }

        public int DesignationId { get; set; }

        public int? ReportingAuthorityId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}

namespace Demo01.MvcSessionCookieDapperSp.ViewModels
{
    public class ReportingUserViewModel
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public string DesignationName { get; set; } = string.Empty;

        public int LevelNo { get; set; }

        public DateTime? LoginTime { get; set; }

        public DateTime? LogoutTime { get; set; }

        public int? TotalDurationMinutes { get; set; }

        public int LunchDeductionMinutes { get; set; }

        public int? NetDurationMinutes { get; set; }
        public DateTime? CurrentActiveLoginTime { get; set; }

        public bool IsCurrentlyLoggedIn { get; set; }
    }
}

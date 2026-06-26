namespace Demo01.MvcSessionCookieDapperSp.Models
{
    public class UserAttendanceLog
    {
        public long AttendanceLogId { get; set; }

        public int? UserId { get; set; }

        public string? Username { get; set; }

        public DateTime? LoginTime { get; set; }

        public DateTime? LogoutTime { get; set; }

        public int? TotalDurationMinutes { get; set; }

        public int LunchDeductionMinutes { get; set; }

        public int? NetDurationMinutes { get; set; }

        public string? LoginIpAddress { get; set; }

        public string? LogoutIpAddress { get; set; }

        public string? UserAgent { get; set; }

        public string LoginStatus { get; set; } = string.Empty;

        public string? FailureReason { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}

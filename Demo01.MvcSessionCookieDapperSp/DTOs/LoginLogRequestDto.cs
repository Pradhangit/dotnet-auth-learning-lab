namespace Demo01.MvcSessionCookieDapperSp.DTOs
{
    public class LoginLogRequestDto
    {
        public int? UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string LoginStatus { get; set; } = string.Empty;

        public string? FailureReason { get; set; }

        public string? LoginIpAddress { get; set; }

        public string? UserAgent { get; set; }
    }
}

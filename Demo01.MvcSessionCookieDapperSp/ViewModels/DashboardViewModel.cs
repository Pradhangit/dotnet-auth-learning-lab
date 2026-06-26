namespace Demo01.MvcSessionCookieDapperSp.ViewModels
{
    public class DashboardViewModel
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public int DesignationId { get; set; }

        public string DesignationName { get; set; } = string.Empty;

        public int LevelNo { get; set; }
        public bool IsAdmin { get; set; }
    }
}

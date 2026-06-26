namespace Demo01.MvcSessionCookieDapperSp.Models
{
    public class Designation
    {
        public int DesignationId { get; set; }

        public int DepartmentId { get; set; }

        public string DesignationName { get; set; } = string.Empty;

        public int LevelNo { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}

namespace Demo01.MvcSessionCookieDapperSp.DTOs
{
    public class OperationResultDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public int? ResultId { get; set; }
    }
}

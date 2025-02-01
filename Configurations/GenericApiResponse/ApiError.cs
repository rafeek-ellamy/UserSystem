namespace Configurations.GenericApiResponse
{
    public class ApiError(string code, string message, string? detail = null)
    {
        public string Code { get; set; } = code;
        public string Message { get; set; } = message;
        public string? Detail { get; set; } = detail;
    }
}

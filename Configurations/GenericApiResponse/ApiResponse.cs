namespace Configurations.GenericApiResponse
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public IEnumerable<ApiError>? Errors { get; set; }

        public ApiResponse(bool success, T data, string message)
        {
            Success = success;
            Data = data;
            Message = message;
            Errors = new List<ApiError>();
        }

        public ApiResponse(bool success, string message)
        {
            Success = success;
            Data = default;
            Message = message;
            Errors = new List<ApiError>();
        }

        public ApiResponse(T data)
        {
            Success = true;
            Data = data;
            Message = string.Empty;
            Errors = new List<ApiError>();
        }

        public ApiResponse(bool success)
        {
            Success = success;
            Data = default;
            Message = string.Empty;
            Errors = new List<ApiError>();
        }
    }
}

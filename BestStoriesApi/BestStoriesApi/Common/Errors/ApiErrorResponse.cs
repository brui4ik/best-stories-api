namespace BestStoriesApi.Common.Errors;

public sealed class ApiErrorResponse
{
    public int StatusCode { get; init; }
    public string Message { get; set; } = "";
    public object? Details { get; set; }
    public string? TraceId { get; set; }
}
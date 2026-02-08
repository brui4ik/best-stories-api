using System.Net;
using System.Net.Mime;
using FluentValidation;

namespace BestStoriesApi.Common.Errors;

public sealed class AppErrorHandler : IAppErrorHandler
{
    public IResult Handle(Exception ex, HttpContext httpContext)
    {
        var traceId = httpContext.TraceIdentifier;

        var response = ex switch
        {
            ValidationException vex => new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Validation failed.",
                Details = vex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()),
            },

            BadHttpRequestException bhre => new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Bad request.",
                Details = new { error = bhre.Message }
            },

            ArgumentException aex => new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = aex.Message,
                Details = new { error = aex.Message }
            },

            HttpRequestException hre => new ApiErrorResponse
            {
                StatusCode = (int)(hre.StatusCode ?? HttpStatusCode.InternalServerError),
                Message = hre.StatusCode == HttpStatusCode.NotFound ? "Not found." : "Upstream request failed.",
                Details = new { error = hre.Message }
            },

            _ => new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Internal Server Error",
                Details = new { error = ex.Message }
            }
        };

        response.TraceId = traceId;

        return Results.Json(
            response,
            contentType: MediaTypeNames.Application.Json,
            statusCode: response.StatusCode);
    }
}

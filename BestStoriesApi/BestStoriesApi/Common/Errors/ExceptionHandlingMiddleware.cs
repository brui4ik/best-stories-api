namespace BestStoriesApi.Common.Errors;

public sealed class ExceptionHandlingMiddleware(IAppErrorHandler handler) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var result = handler.Handle(ex, context);
            await result.ExecuteAsync(context);
        }
    }
}
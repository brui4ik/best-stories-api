namespace BestStoriesApi.Common.Errors;

public interface IAppErrorHandler
{
    IResult Handle(Exception ex, HttpContext httpContext);
}
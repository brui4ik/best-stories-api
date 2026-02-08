using BestStoriesApi.Common;
using BestStoriesApi.Common.Errors;
using Carter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApi()
    .AddApplication()
    .AddHackerNews(builder.Configuration)
    .AddBestStoriesRateLimiting(builder.Configuration);

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRateLimiter();
app.MapCarter();

app.Run();
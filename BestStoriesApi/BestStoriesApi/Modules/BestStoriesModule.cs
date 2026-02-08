using BestStoriesApi.HackerNews.Queries;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BestStoriesApi.Modules;

public sealed class BestStoriesModule : ICarterModule
{
    
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/best-stories", async (
            [FromQuery]int n,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetBestStoriesQuery(n), ct);
            return Results.Ok(result);
        });
    }
}

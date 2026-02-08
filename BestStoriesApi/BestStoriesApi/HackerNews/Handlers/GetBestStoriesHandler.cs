using AutoMapper;
using BestStoriesApi.Clients;
using BestStoriesApi.Configuration;
using BestStoriesApi.HackerNews.Dtos;
using BestStoriesApi.HackerNews.Models;
using BestStoriesApi.HackerNews.Queries;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BestStoriesApi.HackerNews.Handlers;

public sealed class GetBestStoriesHandler(
    HackerNewsClient hackerNewsClient,
    IMemoryCache cache,
    IMapper mapper,
    IOptions<HackerNewsConfiguration> hnConfig)
    : IRequestHandler<GetBestStoriesQuery, List<BestStory>>
{
    public async Task<List<BestStory>> Handle(GetBestStoriesQuery request, CancellationToken ct)
    {
        var ids = await hackerNewsClient.GetBestStoryIdsAsync(ct);

        var firstN = ids.Take(request.N).ToArray();

        var tasks = firstN.Select(id => GetStoryCachedAsync(id, ct));
        var stories = await Task.WhenAll(tasks);

        return stories
            .Where(s => s is not null)
            .Select(mapper.Map<BestStory>)
            .OrderByDescending(s => s.Score)
            .ToList();
    }
    
    private Task<HackerNewsItem?> GetStoryCachedAsync(long id, CancellationToken ct)
    {
        var cacheKey = $"hn:item:{id}";

        return cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(hnConfig.Value.ItemCacheMinutes);

            return await hackerNewsClient.GetItemAsync(id, ct);
        });
    }
}
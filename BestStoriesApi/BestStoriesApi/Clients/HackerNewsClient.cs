using BestStoriesApi.HackerNews.Dtos;

namespace BestStoriesApi.Clients;

public class HackerNewsClient(HttpClient http)
{
    public async Task<long[]> GetBestStoryIdsAsync(CancellationToken ct)
        => await http.GetFromJsonAsync<long[]>("beststories.json", ct) ?? [];

    public async Task<HackerNewsItem?> GetItemAsync(long id, CancellationToken ct)
        => await http.GetFromJsonAsync<HackerNewsItem>($"item/{id}.json", ct);
}
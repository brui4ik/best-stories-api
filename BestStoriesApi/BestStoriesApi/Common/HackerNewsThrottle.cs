using Microsoft.Extensions.Options;
using BestStoriesApi.Configuration;

namespace BestStoriesApi.Common;

public sealed class HackerNewsThrottle
{
    private readonly SemaphoreSlim _semaphore;

    public HackerNewsThrottle(IOptions<HackerNewsConfiguration> options)
    {
        var maxConcurrentRequests = options.Value.MaxConcurrentRequests;
        _semaphore = new SemaphoreSlim(maxConcurrentRequests, maxConcurrentRequests);
    }

    public async Task<T> RunAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken ct)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            return await action(ct);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
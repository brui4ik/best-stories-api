namespace BestStoriesApi.Configuration;

public sealed class HackerNewsConfiguration
{
    public const string SectionName = "HackerNews";
    
    public string BaseUrl { get; init; }
    public long TimeoutSeconds { get; init; }
    public int ItemCacheMinutes { get; init; }
    public int MaxConcurrentRequests { get; init; }
    public RetryOptions Retry { get; init; }
}
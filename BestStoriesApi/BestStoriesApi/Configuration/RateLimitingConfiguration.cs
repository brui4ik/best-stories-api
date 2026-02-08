namespace BestStoriesApi.Configuration;

public class RateLimitingConfiguration
{
    public const string SectionName = "RateLimiting";
    
    public string PolicyName { get; init; }
    public int PermitLimit { get; init; }
    public int WindowMinutes { get; init; }
    public int QueueLimit { get; init; }
}
namespace BestStoriesApi.Configuration;

public sealed class RetryOptions
{
    public int MaxRetries { get; set; }
    public int BaseDelayMs { get; set; }
    public bool UseExponentialBackoff { get; set; }
    public bool UseJitter { get; set; }

    public bool RetryOnTimeout { get; set; }
    public bool RetryOn429 { get; set; }
    public bool RetryOn5xx { get; set; }
}
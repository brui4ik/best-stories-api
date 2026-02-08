namespace BestStoriesApi.Common;

public sealed class HackerNewsThrottleHandler(HackerNewsThrottle throttle) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        => throttle.RunAsync(_ => base.SendAsync(request, ct), ct);
}
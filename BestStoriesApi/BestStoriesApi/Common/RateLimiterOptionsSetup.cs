using System.Threading.RateLimiting;
using BestStoriesApi.Configuration;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace BestStoriesApi.Common;

public sealed class RateLimiterOptionsSetup(IOptions<RateLimitingConfiguration> config)
    : IConfigureOptions<RateLimiterOptions>
{
    private readonly RateLimitingConfiguration _config = config.Value;

    public void Configure(RateLimiterOptions options)
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        options.AddPolicy(_config.PolicyName, context =>
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip";

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: ip,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = _config.PermitLimit,
                    Window = TimeSpan.FromMinutes(_config.WindowMinutes),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = _config.QueueLimit,
                    AutoReplenishment = true
                });
        });
    }
}
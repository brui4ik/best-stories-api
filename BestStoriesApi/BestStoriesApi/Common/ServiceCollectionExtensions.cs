using BestStoriesApi.Clients;
using BestStoriesApi.Common.Errors;
using BestStoriesApi.Configuration;
using BestStoriesApi.HackerNews.Mapping;
using Carter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace BestStoriesApi.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddCarter();
        services.AddSingleton<IAppErrorHandler, AppErrorHandler>();
        services.AddTransient<ExceptionHandlingMiddleware>();
            
        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
        services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddAutoMapper(cfg => { }, typeof(BestStoryProfile));
        return services;
    }

    public static IServiceCollection AddHackerNews(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<HackerNewsConfiguration>()
            .Bind(config.GetSection(HackerNewsConfiguration.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.BaseUrl) && Uri.TryCreate(o.BaseUrl, UriKind.Absolute, out _), "Invalid BaseUrl")
            .Validate(o => o.TimeoutSeconds is > 0 and <= 120, "Invalid TimeoutSeconds")
            .Validate(o => o.ItemCacheMinutes is > 0 and <= 60, "Invalid ItemCacheMinutes")
            .Validate(o => o.MaxConcurrentRequests is > 0 and <= 100, "Invalid MaxConcurrentRequests")
            .Validate(o => o.Retry.MaxRetries is >= 0 and <= 10, "Invalid Retry:MaxRetries")
            .Validate(o => o.Retry.BaseDelayMs is >= 0 and <= 10_000, "Invalid Retry:BaseDelayMs")
            .ValidateOnStart();

        services.AddMemoryCache();
        services.AddTransient<HackerNewsThrottleHandler>();
        services.AddSingleton<HackerNewsThrottle>();

        services.AddHttpClient<HackerNewsClient>((sp, client) =>
            {
                var hackerNewsConfiguration = sp.GetRequiredService<IOptions<HackerNewsConfiguration>>().Value;
                client.BaseAddress = new Uri(hackerNewsConfiguration.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(hackerNewsConfiguration.TimeoutSeconds);
            })
            .AddHttpMessageHandler<HackerNewsThrottleHandler>()
            .AddPolicyHandler((sp, _) =>
            {
                var hackerNewsConfiguration = sp.GetRequiredService<IOptions<HackerNewsConfiguration>>().Value;
                var retryOptions = hackerNewsConfiguration.Retry;

                var builder = HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .OrResult(resp =>
                        (retryOptions.RetryOn429 && (int)resp.StatusCode == 429) ||
                        (retryOptions.RetryOn5xx && (int)resp.StatusCode >= 500));

                if (retryOptions.RetryOnTimeout)
                {
                    builder = builder.Or<TaskCanceledException>();
                }

                return builder.WaitAndRetryAsync(
                    retryCount: retryOptions.MaxRetries,
                    sleepDurationProvider: attempt =>
                    {
                        var baseDelay = TimeSpan.FromMilliseconds(retryOptions.BaseDelayMs);

                        var delay = retryOptions.UseExponentialBackoff
                            ? TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1))
                            : baseDelay;

                        if (retryOptions.UseJitter)
                        {
                            delay += TimeSpan.FromMilliseconds(Random.Shared.Next(0, 101));
                        }

                        return delay;
                    });
            });

        return services;
    }

    public static IServiceCollection AddBestStoriesRateLimiting(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<RateLimitingConfiguration>()
            .Bind(config.GetSection(RateLimitingConfiguration.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.PolicyName), "PolicyName required")
            .Validate(o => o.PermitLimit > 0, "PermitLimit must be > 0")
            .Validate(o => o.WindowMinutes > 0, "WindowMinutes must be > 0")
            .Validate(o => o.QueueLimit >= 0, "QueueLimit must be >= 0")
            .ValidateOnStart();

        services.AddSingleton<IConfigureOptions<RateLimiterOptions>, RateLimiterOptionsSetup>();
        services.AddRateLimiter();

        return services;
    }
}

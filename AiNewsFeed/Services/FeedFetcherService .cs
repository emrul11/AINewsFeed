using System.Text.RegularExpressions;
using System.Web;
using CodeHollow.FeedReader;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using AiNewsFeed.Data;
using AiNewsFeed.DTOs;
using AiNewsFeed.Models;

namespace AiNewsFeed.Services;

public class FeedFetcherService : IFeedFetcherService
{
    private readonly AppDbContext _db;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<FeedFetcherService> _logger;

    public FeedFetcherService(
        AppDbContext db,
        IHttpClientFactory httpClientFactory,
        IConfiguration config,
        ILogger<FeedFetcherService> logger)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
        _config = config;
        _logger = logger;
    }

    public async Task<FeedRefreshResponseDto> RefreshFeedsAsync(CancellationToken cancellationToken = default)
    {
        var delayMs = _config.GetValue<int>("FeedSettings:DelayBetweenRequestsMs", 500);
        var maxFailures = _config.GetValue<int>("FeedSettings:MaxConsecutiveFailures", 5);
        var retentionDays = _config.GetValue<int>("FeedSettings:ArticleRetentionDays", 90);

        var sources = await _db.FeedSources
            .Where(f => f.IsActive)
            .ToListAsync(cancellationToken);

        var results = new List<FeedFetchResult>();
        int totalNewArticles = 0;

        foreach (var source in sources)
        {
            var result = await FetchSingleFeedAsync(source, cancellationToken);
            results.Add(result);

            if (result.Success)
            {
                source.LastFetchedAt = DateTime.UtcNow;
                source.LastFetchStatus = "OK";
                source.ConsecutiveFailures = 0;
                totalNewArticles += result.NewArticlesAdded;
            }
            else
            {
                source.ConsecutiveFailures++;
                source.LastFetchStatus = "Failed";
                source.LastErrorMessage = result.ErrorMessage;

                if (source.ConsecutiveFailures >= maxFailures)
                {
                    source.IsActive = false;
                    source.LastFetchStatus = "Disabled";
                    _logger.LogWarning("Feed source '{SourceName}' disabled after {Failures} consecutive failures.",
                        source.Name, source.ConsecutiveFailures);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);

            // Delay before next request (except for the last one)
            if (source != sources.Last())
            {
                await Task.Delay(delayMs, cancellationToken);
            }
        }

        // Soft-delete articles older than retention period
        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
        var oldArticles = await _db.Articles
            .Where(a => !a.IsDeleted && a.PublishedAt < cutoffDate)
            .ToListAsync(cancellationToken);

        foreach (var article in oldArticles)
        {
            article.IsDeleted = true;
        }

        if (oldArticles.Count > 0)
        {
            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Soft-deleted {Count} articles older than {RetentionDays} days.",
                oldArticles.Count, retentionDays);
        }

        var failedSources = results.Where(r => !r.Success).ToList();
        var disabledSources = sources.Count(s => !s.IsActive);

        _logger.LogInformation(
            "Feed refresh complete. New articles: {New}, Failed sources: {Failed}, Disabled sources: {Disabled}",
            totalNewArticles, failedSources.Count, disabledSources);

        return new FeedRefreshResponseDto
        {
            NewArticlesAdded = totalNewArticles,
            FailedSources = failedSources.Count,
            FailedSourceNames = failedSources.Select(r => r.SourceName).ToList(),
            DisabledSources = disabledSources
        };
    }

    private async Task<FeedFetchResult> FetchSingleFeedAsync(FeedSource source, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient("FeedClient");

        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, ctx) =>
                {
                    _logger.LogWarning(exception,
                        "Retry {RetryCount} for feed '{SourceName}' after {Delay}s",
                        retryCount, timeSpan.TotalSeconds, source.Name);
                });

        try
        {
            var feed = await retryPolicy.ExecuteAsync(async ct =>
            {
                var feedContent = await httpClient.GetStringAsync(source.FeedUrl, ct);
                return FeedReader.ReadFromString(feedContent);
            }, cancellationToken);

            var existingUrls = await _db.Articles
                .Select(a => a.Url)
                .ToListAsync(cancellationToken);

            var existingUrlSet = new HashSet<string>(existingUrls);
            int newArticlesCount = 0;

            foreach (var item in feed.Items.Take(20)) // Limit to latest 20 per feed
            {
                var normalizedUrl = NormalizeUrl(item.Link);

                if (string.IsNullOrWhiteSpace(normalizedUrl) || existingUrlSet.Contains(normalizedUrl))
                    continue;

                var article = new Article
                {
                    Title = item.Title?.Trim() ?? "Untitled",
                    Summary = item.Description?.Trim(),
                    Content = item.Content?.Trim(),
                    Url = normalizedUrl,
                    Source = source.Name,
                    Author = item.Author?.Trim(),
                    PublishedAt = item.PublishingDate ?? DateTime.UtcNow.AddMinutes(-5),
                    FetchedAt = DateTime.UtcNow
                };

                _db.Articles.Add(article);
                existingUrlSet.Add(normalizedUrl);
                newArticlesCount++;
            }

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Fetched '{SourceName}': {NewArticles} new articles.",
                source.Name, newArticlesCount);

            return new FeedFetchResult
            {
                SourceName = source.Name,
                Success = true,
                NewArticlesAdded = newArticlesCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch feed '{SourceName}'", source.Name);
            return new FeedFetchResult
            {
                SourceName = source.Name,
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private static string NormalizeUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        try
        {
            var uri = new Uri(url.Trim());

            // Force HTTPS
            var builder = new UriBuilder(uri)
            {
                Scheme = "https",
                Port = -1 // Use default port for scheme
            };

            // Strip fragment
            builder.Fragment = string.Empty;

            var query = HttpUtility.ParseQueryString(builder.Query);

            // Strip UTM parameters
            var utmParams = query.AllKeys
                .Where(k => k != null && k.StartsWith("utm_", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var param in utmParams)
            {
                query.Remove(param!);
            }

            builder.Query = query.ToString();

            var normalized = builder.ToString()
                .TrimEnd('/')
                .ToLowerInvariant();

            return normalized;
        }
        catch
        {
            return url.Trim();
        }
    }
}
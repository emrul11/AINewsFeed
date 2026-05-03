using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using CodeHollow.FeedReader;
using Microsoft.EntityFrameworkCore;
using Polly;
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

    // Keyword extraction patterns for model/company detection
    private static readonly Dictionary<string, string[]> ModelPatterns = new(StringComparer.OrdinalIgnoreCase)
    {
        ["kimi-k2.6"] = new[] { "kimi k2.6", "kimi-k2.6", "k2.6" },
        ["kimi-k2.5"] = new[] { "kimi k2.5", "kimi-k2.5", "k2.5" },
        ["claude-opus-4.7"] = new[] { "claude opus 4.7", "opus 4.7" },
        ["claude-opus-4.6"] = new[] { "claude opus 4.6", "opus 4.6" },
        ["claude-sonnet-4"] = new[] { "claude sonnet 4", "sonnet 4" },
        ["codex-5.5"] = new[] { "codex 5.5", "codex-5.5" },
        ["gpt-5"] = new[] { "gpt-5", "gpt 5", "o3", "o4" },
        ["gpt-4.5"] = new[] { "gpt-4.5", "gpt 4.5" },
        ["gemini-2.5"] = new[] { "gemini 2.5", "gemini-2.5" },
        ["gemini-ultra"] = new[] { "gemini ultra", "gemini-ultra" },
        ["llama-4"] = new[] { "llama 4", "llama-4" },
        ["mistral-large"] = new[] { "mistral large", "mistral-large" },
        ["grok-3"] = new[] { "grok 3", "grok-3" },
    };

    private static readonly Dictionary<string, string[]> CompanyPatterns = new(StringComparer.OrdinalIgnoreCase)
    {
        ["moonshot"] = new[] { "moonshot", "moonshot ai" },
        ["anthropic"] = new[] { "anthropic" },
        ["openai"] = new[] { "openai", "open ai" },
        ["google"] = new[] { "google", "deepmind", "google deepmind" },
        ["meta"] = new[] { "meta", "facebook" },
        ["microsoft"] = new[] { "microsoft" },
        ["nvidia"] = new[] { "nvidia" },
        ["huggingface"] = new[] { "huggingface", "hugging face" },
        ["stability"] = new[] { "stability ai", "stability" },
        ["cohere"] = new[] { "cohere" },
        ["mistral"] = new[] { "mistral ai", "mistral" },
        ["xai"] = new[] { "xai", "x.ai", "grok" },
    };

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
            .OrderByDescending(f => f.Priority)
            .ToListAsync(cancellationToken);

        var results = new List<FeedFetchResult>();
        int totalNewArticles = 0;

        foreach (var source in sources)
        {
            // Skip if fetched too recently
            if (source.LastFetchedAt.HasValue &&
                DateTime.UtcNow - source.LastFetchedAt.Value < TimeSpan.FromMinutes(source.FetchIntervalMinutes))
            {
                _logger.LogDebug("Skipping '{SourceName}' — fetched {Minutes}m ago",
                    source.Name, (DateTime.UtcNow - source.LastFetchedAt.Value).TotalMinutes);
                continue;
            }

            var result = await FetchSingleSourceAsync(source, cancellationToken);
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

            if (source != sources.Last())
            {
                await Task.Delay(delayMs, cancellationToken);
            }
        }

        // Soft-delete old articles
        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
        var oldArticles = await _db.Articles
            .Where(a => !a.IsDeleted && a.PublishedAt < cutoffDate)
            .ExecuteUpdateAsync(setters => setters.SetProperty(a => a.IsDeleted, true), cancellationToken);

        _logger.LogInformation("Soft-deleted {Count} articles older than {RetentionDays} days.", oldArticles, retentionDays);

        var failedSources = results.Where(r => !r.Success).ToList();

        return new FeedRefreshResponseDto
        {
            NewArticlesAdded = totalNewArticles,
            FailedSources = failedSources.Count,
            FailedSourceNames = failedSources.Select(r => r.SourceName).ToList(),
            DisabledSources = sources.Count(s => !s.IsActive)
        };
    }

    private async Task<FeedFetchResult> FetchSingleSourceAsync(FeedSource source, CancellationToken cancellationToken)
    {
        return source.Type switch
        {
            FeedSourceType.Rss or FeedSourceType.GoogleNews => await FetchRssAsync(source, cancellationToken),
            FeedSourceType.RedditJson => await FetchRedditAsync(source, cancellationToken),
            FeedSourceType.HackerNews => await FetchHackerNewsAsync(source, cancellationToken),
            _ => new FeedFetchResult { SourceName = source.Name, Success = false, ErrorMessage = "Unknown source type" }
        };
    }

    private async Task<FeedFetchResult> FetchRssAsync(FeedSource source, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient("FeedClient");

        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        try
        {
            var feed = await retryPolicy.ExecuteAsync(async ct =>
            {
                var feedContent = await httpClient.GetStringAsync(source.FeedUrl, ct);
                return FeedReader.ReadFromString(feedContent);
            }, cancellationToken);

            var existingUrls = await _db.Articles
                .Where(a => !a.IsDeleted)
                .Select(a => a.Url)
                .ToListAsync(cancellationToken);

            var existingUrlSet = new HashSet<string>(existingUrls);
            int newArticlesCount = 0;

            foreach (var item in feed.Items.Take(30))
            {
                var normalizedUrl = NormalizeUrl(item.Link);
                if (string.IsNullOrWhiteSpace(normalizedUrl) || existingUrlSet.Contains(normalizedUrl))
                    continue;

                var title = item.Title?.Trim() ?? "Untitled";
                var summary = item.Description?.Trim();
                var content = item.Content?.Trim() ?? summary;
                var fullText = $"{title} {summary} {content}";

                // KEYWORD FILTERING
                if (!source.AlwaysInclude && !string.IsNullOrWhiteSpace(source.RequiredKeywords))
                {
                    var keywords = source.RequiredKeywords.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(k => k.Trim().ToLower());
                    var textLower = fullText.ToLower();

                    if (!keywords.Any(k => textLower.Contains(k)))
                        continue;
                }

                // ENTITY EXTRACTION
                var mentionedModels = ExtractMatches(fullText, ModelPatterns);
                var mentionedCompanies = ExtractMatches(fullText, CompanyPatterns);

                var article = new Article
                {
                    Title = title,
                    Summary = summary,
                    Content = content,
                    Url = normalizedUrl,
                    Source = source.Name,
                    Author = item.Author?.Trim(),
                    PublishedAt = item.PublishingDate ?? DateTime.UtcNow.AddMinutes(-5),
                    FetchedAt = DateTime.UtcNow,
                    MentionedModels = JsonSerializer.Serialize(mentionedModels),
                    MentionedCompanies = JsonSerializer.Serialize(mentionedCompanies),
                    ContentHash = ComputeHash(title + summary),
                    SourceType = source.Type,
                    SearchVector = fullText.ToLower()
                };

                _db.Articles.Add(article);
                existingUrlSet.Add(normalizedUrl);
                newArticlesCount++;
            }

            await _db.SaveChangesAsync(cancellationToken);

            return new FeedFetchResult
            {
                SourceName = source.Name,
                Success = true,
                NewArticlesAdded = newArticlesCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch RSS '{SourceName}'", source.Name);
            return new FeedFetchResult { SourceName = source.Name, Success = false, ErrorMessage = ex.Message };
        }
    }

    private async Task<FeedFetchResult> FetchRedditAsync(FeedSource source, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient("FeedClient");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "AiNewsFeed/1.0");

        try
        {
            var json = await httpClient.GetStringAsync(source.FeedUrl, cancellationToken);
            var redditData = JsonSerializer.Deserialize<JsonElement>(json);
            var posts = redditData.GetProperty("data").GetProperty("children");

            var existingUrls = await _db.Articles
                .Where(a => !a.IsDeleted)
                .Select(a => a.Url)
                .ToListAsync(cancellationToken);

            var existingUrlSet = new HashSet<string>(existingUrls);
            int newArticlesCount = 0;

            foreach (var post in posts.EnumerateArray())
            {
                var data = post.GetProperty("data");
                var title = data.GetProperty("title").GetString() ?? "Untitled";
                var url = data.GetProperty("url").GetString() ??
                          $"https://reddit.com{data.GetProperty("permalink").GetString()}";
                var selftext = data.GetProperty("selftext").GetString() ?? "";
                var author = data.GetProperty("author").GetString() ?? "";
                var createdUtc = DateTimeOffset.FromUnixTimeSeconds(
                    data.GetProperty("created_utc").GetInt64()).UtcDateTime;

                var normalizedUrl = NormalizeUrl(url);
                if (existingUrlSet.Contains(normalizedUrl))
                    continue;

                var fullText = $"{title} {selftext}";

                // Keyword filter
                if (!string.IsNullOrWhiteSpace(source.RequiredKeywords))
                {
                    var keywords = source.RequiredKeywords.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(k => k.Trim().ToLower());
                    if (!keywords.Any(k => fullText.ToLower().Contains(k)))
                        continue;
                }

                var mentionedModels = ExtractMatches(fullText, ModelPatterns);
                var mentionedCompanies = ExtractMatches(fullText, CompanyPatterns);

                var article = new Article
                {
                    Title = $"[Reddit] {title}",
                    Summary = selftext.Length > 500 ? selftext[..500] + "..." : selftext,
                    Url = normalizedUrl,
                    Source = source.Name,
                    Author = author,
                    PublishedAt = createdUtc,
                    FetchedAt = DateTime.UtcNow,
                    MentionedModels = JsonSerializer.Serialize(mentionedModels),
                    MentionedCompanies = JsonSerializer.Serialize(mentionedCompanies),
                    ContentHash = ComputeHash(title + selftext),
                    SourceType = source.Type,
                    SearchVector = fullText.ToLower()
                };

                _db.Articles.Add(article);
                existingUrlSet.Add(normalizedUrl);
                newArticlesCount++;
            }

            await _db.SaveChangesAsync(cancellationToken);
            return new FeedFetchResult { SourceName = source.Name, Success = true, NewArticlesAdded = newArticlesCount };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch Reddit '{SourceName}'", source.Name);
            return new FeedFetchResult { SourceName = source.Name, Success = false, ErrorMessage = ex.Message };
        }
    }

    private async Task<FeedFetchResult> FetchHackerNewsAsync(FeedSource source, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient("FeedClient");

        try
        {
            var json = await httpClient.GetStringAsync(source.FeedUrl, cancellationToken);
            var hnData = JsonSerializer.Deserialize<JsonElement>(json);
            var hits = hnData.GetProperty("hits");

            var existingUrls = await _db.Articles
                .Where(a => !a.IsDeleted)
                .Select(a => a.Url)
                .ToListAsync(cancellationToken);

            var existingUrlSet = new HashSet<string>(existingUrls);
            int newArticlesCount = 0;

            foreach (var hit in hits.EnumerateArray())
            {
                var title = hit.GetProperty("title").GetString() ?? "Untitled";
                var url = hit.GetProperty("url").GetString() ??
                          $"https://news.ycombinator.com/item?id={hit.GetProperty("objectID").GetString()}";
                var author = hit.TryGetProperty("author", out var a) ? a.GetString() : null;
                var createdAt = hit.TryGetProperty("created_at", out var ca)
                    ? DateTime.Parse(ca.GetString()!)
                    : DateTime.UtcNow;

                var normalizedUrl = NormalizeUrl(url);
                if (existingUrlSet.Contains(normalizedUrl))
                    continue;

                var fullText = title;

                var mentionedModels = ExtractMatches(fullText, ModelPatterns);
                var mentionedCompanies = ExtractMatches(fullText, CompanyPatterns);

                var article = new Article
                {
                    Title = $"[HN] {title}",
                    Url = normalizedUrl,
                    Source = source.Name,
                    Author = author,
                    PublishedAt = createdAt,
                    FetchedAt = DateTime.UtcNow,
                    MentionedModels = JsonSerializer.Serialize(mentionedModels),
                    MentionedCompanies = JsonSerializer.Serialize(mentionedCompanies),
                    ContentHash = ComputeHash(title),
                    SourceType = source.Type,
                    SearchVector = fullText.ToLower()
                };

                _db.Articles.Add(article);
                existingUrlSet.Add(normalizedUrl);
                newArticlesCount++;
            }

            await _db.SaveChangesAsync(cancellationToken);
            return new FeedFetchResult { SourceName = source.Name, Success = true, NewArticlesAdded = newArticlesCount };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch HN '{SourceName}'", source.Name);
            return new FeedFetchResult { SourceName = source.Name, Success = false, ErrorMessage = ex.Message };
        }
    }

    // Extract which models/companies are mentioned in text
    private static List<string> ExtractMatches(string text, Dictionary<string, string[]> patterns)
    {
        var textLower = text.ToLower();
        var matches = new List<string>();

        foreach (var (key, variants) in patterns)
        {
            if (variants.Any(v => textLower.Contains(v.ToLower())))
                matches.Add(key);
        }

        return matches;
    }

    private static string ComputeHash(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input.ToLowerInvariant().Trim());
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash)[..16];
    }

    private static string NormalizeUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        try
        {
            var uri = new Uri(url.Trim());
            var builder = new UriBuilder(uri) { Scheme = "https", Port = -1 };
            builder.Fragment = string.Empty;

            var query = HttpUtility.ParseQueryString(builder.Query);
            var utmParams = query.AllKeys
                .Where(k => k != null && k.StartsWith("utm_", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var param in utmParams)
                query.Remove(param!);

            builder.Query = query.ToString();
            return builder.ToString().TrimEnd('/').ToLowerInvariant();
        }
        catch
        {
            return url.Trim().ToLowerInvariant();
        }
    }
}
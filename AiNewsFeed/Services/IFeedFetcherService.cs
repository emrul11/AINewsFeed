using AiNewsFeed.DTOs;

namespace AiNewsFeed.Services;

public interface IFeedFetcherService
{
    Task<FeedRefreshResponseDto> RefreshFeedsAsync(CancellationToken cancellationToken = default);
}
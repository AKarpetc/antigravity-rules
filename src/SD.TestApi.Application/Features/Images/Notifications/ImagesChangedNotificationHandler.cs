using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace SD.TestApi.Application.Features.Images.Notifications;

internal class ImagesChangedNotificationHandler : INotificationHandler<ImagesChangedNotification>
{
    private readonly IDistributedCache _cache;
    private const string CacheKey = "Images.All";

    public ImagesChangedNotificationHandler(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task Handle(ImagesChangedNotification notification, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(CacheKey, cancellationToken);
    }
}

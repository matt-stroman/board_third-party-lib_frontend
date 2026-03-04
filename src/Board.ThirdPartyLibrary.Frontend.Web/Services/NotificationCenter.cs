namespace Board.ThirdPartyLibrary.Frontend.Web.Services;

/// <summary>
/// Provides the current user's in-app notifications to interactive web UI components.
/// </summary>
public interface INotificationCenter
{
    /// <summary>
    /// Raised when notification state changes.
    /// </summary>
    event Action? Changed;

    /// <summary>
    /// Gets the notifications currently cached for the active session.
    /// </summary>
    IReadOnlyList<UserNotification> Notifications { get; }

    /// <summary>
    /// Gets the number of unread notifications in the current cache.
    /// </summary>
    int UnreadCount { get; }

    /// <summary>
    /// Gets a value indicating whether notifications have been loaded at least once.
    /// </summary>
    bool IsLoaded { get; }

    /// <summary>
    /// Loads notifications once for the active session when needed.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task EnsureLoadedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes notifications from the backend API.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RefreshAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a notification as read and updates the local cache.
    /// </summary>
    /// <param name="notificationId">Notification identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task MarkReadAsync(Guid notificationId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Default in-memory notification cache for the current web session.
/// </summary>
/// <param name="apiClient">Backend API client.</param>
public sealed class NotificationCenter(IBoardLibraryApiClient apiClient) : INotificationCenter
{
    private IReadOnlyList<UserNotification> notifications = [];

    /// <inheritdoc />
    public event Action? Changed;

    /// <inheritdoc />
    public IReadOnlyList<UserNotification> Notifications => notifications;

    /// <inheritdoc />
    public int UnreadCount => notifications.Count(static notification => !notification.IsRead);

    /// <inheritdoc />
    public bool IsLoaded { get; private set; }

    /// <inheritdoc />
    public async Task EnsureLoadedAsync(CancellationToken cancellationToken = default)
    {
        if (IsLoaded)
        {
            return;
        }

        await RefreshAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        notifications = (await apiClient.GetNotificationsAsync(cancellationToken))
            .Notifications
            .OrderBy(static notification => notification.IsRead)
            .ThenByDescending(static notification => notification.CreatedAt)
            .ToArray();
        IsLoaded = true;
        Changed?.Invoke();
    }

    /// <inheritdoc />
    public async Task MarkReadAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var response = await apiClient.MarkNotificationReadAsync(notificationId, cancellationToken);
        notifications = notifications
            .Select(notification => notification.NotificationId == notificationId ? response.Notification : notification)
            .OrderBy(static notification => notification.IsRead)
            .ThenByDescending(static notification => notification.CreatedAt)
            .ToArray();
        IsLoaded = true;
        Changed?.Invoke();
    }
}

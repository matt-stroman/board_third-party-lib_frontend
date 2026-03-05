namespace Board.ThirdPartyLibrary.Frontend.Web.Services;

/// <summary>
/// Shared state container for the authenticated user's profile in the current web session.
/// </summary>
public interface IUserProfileState
{
    /// <summary>
    /// Raised when the current user profile value changes.
    /// </summary>
    event Action? Changed;

    /// <summary>
    /// Gets the most recently known user profile.
    /// </summary>
    UserProfile? Profile { get; }

    /// <summary>
    /// Sets the current user profile and notifies subscribers.
    /// </summary>
    /// <param name="profile">Updated profile snapshot.</param>
    void SetProfile(UserProfile? profile);
}

/// <summary>
/// Default in-memory <see cref="IUserProfileState" /> implementation.
/// </summary>
internal sealed class UserProfileState : IUserProfileState
{
    /// <inheritdoc />
    public event Action? Changed;

    /// <inheritdoc />
    public UserProfile? Profile { get; private set; }

    /// <inheritdoc />
    public void SetProfile(UserProfile? profile)
    {
        Profile = profile;
        Changed?.Invoke();
    }
}

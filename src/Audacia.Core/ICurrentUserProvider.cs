namespace Audacia.Core;

/// <summary>
/// Represents a type that can access the currently authenticated user.
/// </summary>
/// <typeparam name="TId">The type of the user Id.</typeparam>
public interface ICurrentUserProvider<TId>
{
    /// <summary>
    /// Gets the Id of the current user.
    /// </summary>
    /// <returns>The current user's Id.</returns>
    TId GetCurrentUserId();
}

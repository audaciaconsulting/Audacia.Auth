using System;

namespace Audacia.Auth.OpenIddict.Token;

/// <summary>
/// Represents an exception that occurs when an 'invalid_grant' response must be returned to the client.
/// </summary>
public class InvalidGrantException : Exception
{
    /// <summary>
    /// Initializes an instance of <see cref="InvalidGrantException"/>.
    /// </summary>
    public InvalidGrantException()
    {
    }

    /// <summary>
    /// Initializes an <see cref="InvalidGrantException"/> with the given <paramref name="message"/>.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public InvalidGrantException(string message) : base(message)
    {
    }
    
    /// <summary>
    /// Initializes an instance of <see cref="InvalidGrantException"/>
    /// with the given <paramref name="message"/> and <paramref name="innerException"/>.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">An <see cref="Exception"/> object to wrap.</param>
    public InvalidGrantException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

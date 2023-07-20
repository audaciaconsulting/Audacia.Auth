using System;

namespace Audacia.Auth.OpenIddict.DependencyInjection;

/// <summary>
/// An class that represents an exception during the configuration of OpenIddict.
/// </summary>
public class OpenIddictConfigurationException : Exception
{
    /// <summary>
    /// Initializes an instance of <see cref="OpenIddictConfigurationException"/>.
    /// </summary>
    public OpenIddictConfigurationException()
    {
    }

    /// <summary>
    /// Initializes an instance of <see cref="OpenIddictConfigurationException"/>
    /// with the given <paramref name="message"/>.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public OpenIddictConfigurationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes an instance of <see cref="OpenIddictConfigurationException"/>
    /// with the given <paramref name="message"/> and <paramref name="innerException"/>.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">An <see cref="Exception"/> object to wrap.</param>
    public OpenIddictConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

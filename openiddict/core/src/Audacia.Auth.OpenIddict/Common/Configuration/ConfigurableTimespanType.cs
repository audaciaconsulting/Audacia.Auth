namespace Audacia.Auth.OpenIddict.Common.Configuration
{
    /// <summary>
    /// An enum that is used to give the type for a configurable timespan
    /// to ensure that the user is providing from the set list.
    /// </summary>
    public enum ConfigurableTimespanType
    {
        /// <summary>
        /// No value has been given.
        /// </summary>
        None = 0,

        /// <summary>
        /// Days.
        /// </summary>
        Days = 1,

        /// <summary>
        /// Hours.
        /// </summary>
        Hours = 2,

        /// <summary>
        /// Minutes.
        /// </summary>
        Minutes = 3,

        /// <summary>
        /// Mins (Minutes).
        /// </summary>
        Mins = 4,

        /// <summary>
        /// Seconds.
        /// </summary>
        Seconds = 5,

        /// <summary>
        /// Secs (Seconds).
        /// </summary>
        Secs = 6
    }
}
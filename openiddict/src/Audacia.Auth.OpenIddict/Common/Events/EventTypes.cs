namespace Audacia.Auth.OpenIddict.Common.Events
{
    /// <summary>
    /// Indicates if the event is a success or a failure.
    /// </summary>
    public enum EventTypes
    {
        /// <summary>
        /// No event type.
        /// </summary>
        None = 0,

        /// <summary>
        /// Success event.
        /// </summary>
        Success = 1,

        /// <summary>
        /// Failure event.
        /// </summary>
        Failure = 2,

        /// <summary>
        /// Information event.
        /// </summary>
        Information = 3,
        
        /// <summary>
        /// Error event.
        /// </summary>
        Error = 4
    }
}

namespace Audacia.Auth.OpenIddict.Common.Events
{
    /// <summary>
    /// Constants for valid event Ids.
    /// </summary>
    public static class EventIds
    {
        /*
        // Authentication related events
        */
        private const int AuthenticationEventsStart = 1000;

        /// <summary>
        /// Successful user login.
        /// </summary>
        public const int UserLoginSuccess = AuthenticationEventsStart + 0;
        
        /// <summary>
        /// Unsuccessful user login.
        /// </summary>
        public const int UserLoginFailure = AuthenticationEventsStart + 1;
        
        /// <summary>
        /// Successful user logout.
        /// </summary>
        public const int UserLogoutSuccess = AuthenticationEventsStart + 2;

        /*
        // Token related events
        */
        private const int TokenEventsStart = 2000;

        /// <summary>
        /// Successful token issue.
        /// </summary>
        public const int TokenIssuedSuccess = TokenEventsStart + 0;

        /// <summary>
        /// Unsuccessful token issue.
        /// </summary>
        public const int TokenIssuedFailure = TokenEventsStart + 1;

        /*
        // Error related events
        */
        private const int ErrorEventsStart = 3000;

        /// <summary>
        /// Unhandled exception.
        /// </summary>
        public const int UnhandledException = ErrorEventsStart + 0;

        /*
        // Grants related events
        */
        private const int GrantsEventsStart = 4000;

        /// <summary>
        /// Consent granted.
        /// </summary>
        public const int ConsentGranted = GrantsEventsStart + 0;
        
        /// <summary>
        /// Consent denied.
        /// </summary>
        public const int ConsentDenied = GrantsEventsStart + 1;
    }
}

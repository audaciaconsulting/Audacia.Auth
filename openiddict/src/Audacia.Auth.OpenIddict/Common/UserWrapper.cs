using System;

namespace Audacia.Auth.OpenIddict.Common
{
    /// <summary>
    /// An object that wraps an underlying <typeparamref name="TUser"/> object and also provides access to the user's ID.
    /// </summary>
    /// <typeparam name="TUser">The type of user.</typeparam>
    /// <typeparam name="TId">The type of the user's ID.</typeparam>
    internal class UserWrapper<TUser, TId> 
        where TUser : class
        where TId : IEquatable<TId>
    {
        /// <summary>
        /// Sets a delegate that can get the user's ID.
        /// </summary>
        internal static Func<TUser, TId>? UserIdGetter { private get; set; }

        /// <summary>
        /// Gets the underlying user object.
        /// </summary>
        internal TUser User { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserWrapper{TUser, TId}"/> class.
        /// </summary>
        /// <param name="user">The <typeparamref name="TUser"/> to wrap.</param>
        internal UserWrapper(TUser user)
        {
            User = user;
        }

        /// <summary>
        /// Gets the ID of the current user.
        /// </summary>
        /// <returns>The user's ID.</returns>
        /// <exception cref="InvalidOperationException">If the <see cref="UserIdGetter"/> has not been set.</exception>
        internal TId GetId()
        {
            if (UserIdGetter == null)
            {
                throw new InvalidOperationException($"The {nameof(UserIdGetter)} property must be set.");
            }

            return UserIdGetter(User);
        }

        public static implicit operator TUser(UserWrapper<TUser, TId> proxy) => proxy.User;
    }
}

using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Audacia.Auth.OpenIddict.DependencyInjection
{
    /// <summary>
    /// Extensions to the <see cref="IMvcBuilder"/> type.
    /// </summary>
    public static class MvcBuilderExtensions
    {
        /// <summary>
        /// Adds the OpenIddict-specific controllers.
        /// </summary>
        /// <typeparam name="TUser">The user type.</typeparam>
        /// <typeparam name="TKey">The user's primary key type.</typeparam>
        /// <param name="mvcBuilder">The <see cref="IMvcBuilder"/> instance to which to add the configuration.</param>
        /// <returns>The given <paramref name="mvcBuilder"/>.</returns>
        public static IMvcBuilder ConfigureOpenIddict<TUser, TKey>(this IMvcBuilder mvcBuilder)
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
        {
            return mvcBuilder.ConfigureApplicationPartManager(manager =>
                manager.FeatureProviders.Add(
                    new OpenIddictControllerFeatureProvider<TUser, TKey>()));
        }
    }
}

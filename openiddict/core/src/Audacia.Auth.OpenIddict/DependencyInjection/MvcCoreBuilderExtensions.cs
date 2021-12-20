using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Audacia.Auth.OpenIddict.DependencyInjection
{
    /// <summary>
    /// Extensions to the <see cref="IMvcBuilder"/> type.
    /// </summary>
    public static class MvcCoreBuilderExtensions
    {
        /// <summary>
        /// Adds the OpenIddict-specific controllers.
        /// </summary>
        /// <typeparam name="TUser">The user type.</typeparam>
        /// <typeparam name="TKey">The user's primary key type.</typeparam>
        /// <param name="mvcCoreBuilder">The <see cref="IMvcCoreBuilder"/> instance to which to add the configuration.</param>
        /// <returns>The given <paramref name="mvcCoreBuilder"/>.</returns>
        public static IMvcCoreBuilder ConfigureOpenIddict<TUser, TKey>(this IMvcCoreBuilder mvcCoreBuilder)
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
        {
            return mvcCoreBuilder.ConfigureApplicationPartManager(manager =>
                manager.FeatureProviders.Add(
                    new OpenIddictControllerFeatureProvider<TUser, TKey>()));
        }
    }
}

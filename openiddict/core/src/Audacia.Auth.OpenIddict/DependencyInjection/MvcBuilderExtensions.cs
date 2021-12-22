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
        /// <typeparam name="TId">The user's primary key type.</typeparam>
        /// <param name="mvcBuilder">The <see cref="IMvcBuilder"/> instance to which to add the configuration.</param>
        /// <returns>The given <paramref name="mvcBuilder"/>.</returns>
        public static IMvcBuilder ConfigureOpenIddict<TUser, TId>(this IMvcBuilder mvcBuilder)
            where TUser : class
        {
            return mvcBuilder.ConfigureApplicationPartManager(manager =>
                manager.FeatureProviders.Add(
                    new OpenIddictControllerFeatureProvider<TUser, TId>()));
        }
    }
}

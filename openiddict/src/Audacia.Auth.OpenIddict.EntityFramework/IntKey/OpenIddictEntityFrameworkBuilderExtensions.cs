using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Audacia.Auth.OpenIddict.EntityFramework.IntKey
{
    /// <summary>
    /// Extensions to the <see cref="OpenIddictEntityFrameworkBuilder"/> type specific to <see cref="int"/> keys.
    /// </summary>
    public static class OpenIddictEntityFrameworkBuilderExtensions
    {
        /// <summary>
        /// Configures OpenIddict to use the specified entities, derived from the default OpenIddict Entity Framework 6.x entities.
        /// </summary>
        /// <param name="builder">The <see cref="OpenIddictEntityFrameworkBuilder"/> instance to configure.</param>
        /// <returns>The given <paramref name="builder"/>.</returns>
        public static OpenIddictEntityFrameworkBuilder ReplaceDefaultEntities([NotNull] this OpenIddictEntityFrameworkBuilder builder) =>
            builder.ReplaceDefaultEntities<
                OpenIddictEntityFrameworkApplication,
                OpenIddictEntityFrameworkAuthorization,
                OpenIddictEntityFrameworkScope,
                OpenIddictEntityFrameworkToken,
                int>();
    }
}

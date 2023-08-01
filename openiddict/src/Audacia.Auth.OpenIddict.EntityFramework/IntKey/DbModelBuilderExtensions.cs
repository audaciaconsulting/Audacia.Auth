using System.Data.Entity;

namespace Audacia.Auth.OpenIddict.EntityFramework.IntKey;

/// <summary>
/// Extensions to the <see cref="DbModelBuilder"/> type related to <see cref="int"/> entity keys.
/// </summary>
public static class DbModelBuilderExtensions
{
    /// <summary>
    /// Adds OpenIddict to the given <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="DbModelBuilder"/> instance to which to add OpenIddict.</param>
    /// <returns>The given <paramref name="builder"/>.</returns>
    public static DbModelBuilder UseOpenIddict(this DbModelBuilder builder)
    {
        return builder.UseOpenIddict<
            OpenIddictEntityFrameworkApplication,
            OpenIddictEntityFrameworkAuthorization,
            OpenIddictEntityFrameworkScope,
            OpenIddictEntityFrameworkToken,
            int>();
    }
}

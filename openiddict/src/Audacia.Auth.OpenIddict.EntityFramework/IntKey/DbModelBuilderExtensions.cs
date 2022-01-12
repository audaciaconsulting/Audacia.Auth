using System.Data.Entity;

namespace Audacia.Auth.OpenIddict.EntityFramework.IntKey
{
    public static class DbModelBuilderExtensions
    {
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
}

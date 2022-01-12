using System;
using System.Data.Entity;

namespace Audacia.Auth.OpenIddict.EntityFramework
{
    /// <summary>
    /// Generic extensions to <see cref="DbModelBuilder"/>.
    /// </summary>
    public static class DbModelBuilderExtensions
    {
        /// <summary>
        /// Adds OpenIddict to the given <paramref name="builder"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the primary key for OpenIddict entities.</typeparam>
        /// <param name="builder">The <see cref="DbModelBuilder"/> instance to which to add OpenIddict.</param>
        /// <returns>The given <paramref name="builder"/>.</returns>
        /// <exception cref="NotSupportedException">When <typeparamref name="TKey"/> is not one of <see cref="int"/>, <see cref="string"/> or <see cref="Guid"/>.</exception>
        public static DbModelBuilder UseOpenIddict<TKey>(this DbModelBuilder builder)
            where TKey : IEquatable<TKey>
        {
            if (typeof(TKey) == typeof(string))
            {
                return builder.UseOpenIddict();
            }

            if (typeof(TKey) == typeof(int))
            {
                return IntKey.DbModelBuilderExtensions.UseOpenIddict(builder);
            }

            if (typeof(TKey) == typeof(Guid))
            {
                return GuidKey.DbModelBuilderExtensions.UseOpenIddict(builder);
            }

            throw new NotSupportedException($"Primary key type {typeof(TKey).Name} is not supported.");
        }
    }
}

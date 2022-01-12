using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Audacia.Auth.OpenIddict.Seeding.EntityFrameworkCore
{
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Registered in dependency injection.")]
    internal class OpenIddictContext<TKey> : DbContext
        where TKey : IEquatable<TKey>
    {
        public OpenIddictContext(DbContextOptions<OpenIddictContext<TKey>> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseOpenIddict<TKey>();
            base.OnModelCreating(modelBuilder);
        }
    }
}

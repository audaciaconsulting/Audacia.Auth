using System;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using Audacia.Auth.OpenIddict.EntityFramework;

namespace Audacia.Auth.OpenIddict.Seeding.EntityFramework
{
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Registered in dependency injection.")]
    internal class OpenIddictContext<TKey> : DbContext
        where TKey : IEquatable<TKey>
    {
        public OpenIddictContext(string connectionString)
            : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.UseOpenIddict<TKey>();
            base.OnModelCreating(modelBuilder);
        }
    }
}

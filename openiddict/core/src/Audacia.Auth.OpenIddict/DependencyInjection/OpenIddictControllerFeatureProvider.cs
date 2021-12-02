using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Audacia.Auth.OpenIddict.DependencyInjection
{
    /// <summary>
    /// Implementation of <see cref="IApplicationFeatureProvider"/> that registers the OpenIddict controllers.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <typeparam name="TKey">The type of the user's primary key.</typeparam>
    internal class OpenIddictControllerFeatureProvider<TUser, TKey> : IApplicationFeatureProvider<ControllerFeature>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Adds the generic <see cref="AuthorizationController{TUser, TKey}"/> and <see cref="UserInfoController{TUser, TKey}"/>
        /// types as controllers.
        /// </summary>
        /// <param name="parts">The <see cref="IEnumerable{ApplicationPart}"/>.</param>
        /// <param name="feature">The <see cref="ControllerFeature"/>.</param>
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, [NotNull]ControllerFeature feature)
        {
            feature.Controllers.Add(typeof(AuthorizationController<TUser, TKey>).GetTypeInfo());
            feature.Controllers.Add(typeof(UserInfoController<TUser, TKey>).GetTypeInfo());
        }
    }
}

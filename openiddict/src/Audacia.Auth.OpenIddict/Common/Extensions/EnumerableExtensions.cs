using System.Collections.Generic;
using System.Linq;

namespace Audacia.Auth.OpenIddict.Common.Extensions
{
    /// <summary>
    /// Extensions to the <see cref="IEnumerable{T}"/> type.
    /// </summary>
    internal static class EnumerableExtensions
    {
        /// <summary>
        /// Swaps a <see langword="null"/> <see cref="IEnumerable{T}"/> for an empty one.
        /// </summary>
        /// <typeparam name="T">The type contained in the given <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to check for <see langword="null"/>.</param>
        /// <returns>An empty <see cref="IEnumerable{T}"/> instance if <paramref name="source"/> is <see langword="null"/>; otherwise returns <paramref name="source"/> unmodified.</returns>
        internal static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source) =>
            source ?? Enumerable.Empty<T>();
    }
}

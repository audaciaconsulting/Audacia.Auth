using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Audacia.Auth.OpenIddict.Common.Extensions;

/// <summary>
/// Extensions to the <see cref="IAsyncEnumerable{T}"/> type.
/// </summary>
public static class AsyncEnumerableExtensions
{
    /// <summary>
    /// Asynchronously converts the given <paramref name="source"/> to a <see cref="List{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type contained within the <paramref name="source"/>.</typeparam>
    /// <param name="source">The <see cref="IAsyncEnumerable{T}"/> to convert to a <see cref="List{T}"/>.</param>
    /// <returns>A <see cref="Task{TResult}"/> which, when completed, contains the data from the given <paramref name="source"/> in a <see cref="List{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="source"/> is <see langword="null"/>.</exception>
    public static Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return ExecuteAsync();

        async Task<List<T>> ExecuteAsync()
        {
            var list = new List<T>();

            await foreach (var element in source)
            {
                list.Add(element);
            }

            return list;
        }
    }
}

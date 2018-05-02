using System;
using System.Collections.Generic;
using System.Linq;

namespace Spike.Support.Shared.Models
{
    public static class NavItemExtensions
    {
        /// Traverses an object hierarchy and return a flattened list of elements
        /// based on a predicate.
        /// <typeparam name="TSource">The type of object in your collection.</typeparam>
        /// <param name="source">The collection of your topmost TSource objects.</param>
        /// <param name="selectorFunction">A predicate for choosing the objects you want.</param>
        /// <param name="getChildrenFunction">A function that fetches the child collection from an object.</param>
        /// <returns>
        ///     A flattened list of objects which meet the criteria in selectorFunction.
        /// </returns>
        public static IEnumerable<TSource> Map<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> selectorFunction,
            Func<TSource, IEnumerable<TSource>> getChildrenFunction) 
        {
            // Add what we have to the stack
            var sourceList = source.ToList();
            var flattenedList = sourceList.Where(selectorFunction);

            // Go through the input enumerable looking for children,
            // and add those if we have them
            foreach (var element in sourceList)
                flattenedList = flattenedList.Concat(
                    getChildrenFunction(element).Map(selectorFunction,
                        getChildrenFunction)
                );
            return flattenedList;
        }
    }
}
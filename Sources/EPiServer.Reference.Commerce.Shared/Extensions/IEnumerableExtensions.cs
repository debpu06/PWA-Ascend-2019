using System.Collections;
using System.Linq;

namespace EPiServer.Reference.Commerce.Shared.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Returns the first elment of the specified type in the list
        /// </summary>
        /// <typeparam name="TType">The type to return.</typeparam>
        /// <param name="list">The list to operate on.</param>
        /// <returns></returns>
        public static TType FirstOfType<TType>(this IEnumerable list)
        {
            return list.OfType<TType>().FirstOrDefault();
        }
    }
}

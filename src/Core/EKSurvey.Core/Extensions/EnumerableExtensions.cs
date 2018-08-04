using System.Collections.Generic;
using System.Linq;

namespace EKSurvey.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsUnanimous<T>(this IEnumerable<T> collection, IEqualityComparer<T> comparer = null)
        {
            if (comparer == null)
                comparer = EqualityComparer<T>.Default;

            var distinction = collection.Distinct(comparer);

            return distinction.Count() == 1;
        }
    }
}

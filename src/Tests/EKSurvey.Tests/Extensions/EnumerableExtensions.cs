using System;
using System.Collections.Generic;
using System.Linq;

namespace EKSurvey.Tests.Extensions
{
    public static class EnumerableExtensions
    {
        private static readonly Random Generator = new Random();

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
        {
            var result = new List<T>();

            using (var iterator = collection.GetEnumerator())
            {
                while (iterator.MoveNext())
                {
                    var j = result.Any() ? Generator.Next(0, result.Count + 1) : 0;
                    if (j == result.Count)
                    {
                        result.Add(iterator.Current);
                    }
                    else
                    {
                        result.Add(result[j]);
                        result[j] = iterator.Current;
                    }
                }
            }
            return result;
        }
    }
}

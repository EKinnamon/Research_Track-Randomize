using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

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

        public static IEnumerable<T> CacheAs<T>(this IEnumerable<T> collection, string cacheFilePath)
        {
            var cacheFileDirectoryName = Path.GetDirectoryName(cacheFilePath);
            Directory.CreateDirectory(cacheFileDirectoryName);

            var serializer = new JsonSerializer
            {
                Formatting = Formatting.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            using (var file = File.CreateText(cacheFilePath))
                serializer.Serialize(file, collection);

            return collection;
        }

        public static IEnumerable<T> FixtureCallback<T>(this IEnumerable<T> collection, Action<IEnumerable<T>> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(collection);

            return collection;
        }
    }
}

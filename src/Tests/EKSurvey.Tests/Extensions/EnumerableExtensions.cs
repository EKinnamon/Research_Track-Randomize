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
            if (string.IsNullOrWhiteSpace(cacheFilePath))
                throw new ArgumentNullException(nameof(cacheFilePath));

            string cacheFileDirectoryName = Path.GetDirectoryName(cacheFilePath) ?? throw new ArgumentNullException(nameof(cacheFileDirectoryName));
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

        public static IEnumerable<T> Apply<T>(this IEnumerable<T> collection, Action<IEnumerable<T>> action)
        {
            action.Invoke(collection);
            return collection;
        }
    }
}

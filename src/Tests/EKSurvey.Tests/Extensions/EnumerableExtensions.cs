using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace EKSurvey.Tests.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> CacheAs<T>(this IEnumerable<T> collection, string cacheFilePath)
        {
            if (string.IsNullOrWhiteSpace(cacheFilePath))
                throw new ArgumentNullException(nameof(cacheFilePath));

            string cacheFileDirectoryName = Path.GetDirectoryName(cacheFilePath) ?? throw new ArgumentNullException(nameof(cacheFileDirectoryName));
            Directory.CreateDirectory(cacheFileDirectoryName);

            var serializer = new JsonSerializer
            {
                Formatting = Formatting.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto
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

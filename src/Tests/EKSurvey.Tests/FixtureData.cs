using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EKSurvey.Tests
{
    public class FixtureData<T> : FixtureData, IEnumerable<T>
    {
        private readonly Lazy<IList<T>> _collection;

        public IEnumerator<T> GetEnumerator() => _collection.Value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public FixtureData(string dataPath) : base(dataPath)
        {
            _collection = new Lazy<IList<T>>(() => Load(JsonArray));
        }

        private static IList<T> Load(JToken jsonObject)
        {
            return jsonObject.ToObject<IList<T>>();
        }
    }

    public class FixtureData
    {
        private readonly string _fileDataRaw;
        private readonly Lazy<JArray> _array;

        public JArray JsonArray => _array.Value;

        public FixtureData(string dataPath)
        {
            _array = new Lazy<JArray>(() => LoadJson(_fileDataRaw));
            using (var reader = new StreamReader(dataPath))
            {
                _fileDataRaw = reader.ReadToEnd();
            }
        }

        private static JArray LoadJson(string jsonData)
        {
            return JsonConvert.DeserializeObject<JArray>(jsonData);
        }
    }
}

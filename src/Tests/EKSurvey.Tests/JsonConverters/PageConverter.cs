using System;
using EKSurvey.Core.Models.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EKSurvey.Tests.JsonConverters
{
    public class PageConverter : JsonConverter<IPage>
    {
        public override void WriteJson(JsonWriter writer, IPage value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IPage ReadJson(JsonReader reader, Type objectType, IPage existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);

            var type = jObject["Discriminator"];

            throw new NotImplementedException();
        }
    }
}

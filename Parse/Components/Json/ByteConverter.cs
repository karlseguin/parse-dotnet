using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Parse
{
   internal class ByteConverterr : JsonConverter
   {
      public override bool CanConvert(Type type)
      {
         return typeof(IEnumerable<byte>).IsAssignableFrom(type);
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
         serializer.Serialize(writer, new Dictionary<string, object>
                                      {
                                         {"__type", "Bytes"},
                                         {"base64", Convert.ToBase64String(((IEnumerable<byte>) value).ToArray())}
                                      });
      }

      public override object ReadJson(JsonReader reader, Type type, object existingValue, JsonSerializer serializer)
      {
         return serializer.Deserialize(reader, type);
      }
   }
}
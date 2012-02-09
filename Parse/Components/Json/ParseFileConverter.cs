using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Parse
{
   public class ParseFileConverter : JsonConverter
   {
      public override bool CanConvert(Type type)
      {
         return (typeof(ParseFile).IsAssignableFrom(type));
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
         var file = (ParseFile)value;
         serializer.Serialize(writer, new Dictionary<string, object>
                                      {
                                         {"__type", "File"},
                                         {"name", file.Name},
                                      });
      }

      public override object ReadJson(JsonReader reader, Type type, object existingValue, JsonSerializer serializer)
      {
         return serializer.Deserialize(reader, type);
      }
   }
}
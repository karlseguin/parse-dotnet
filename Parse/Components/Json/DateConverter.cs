using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Parse
{
   internal class DateConverter : JsonConverter
   {
      public override bool CanConvert(Type type)
      {
         return (typeof(DateTime).IsAssignableFrom(type));
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
         serializer.Serialize(writer, new Dictionary<string, object>
                                      {
                                         {"__type", "Date"},
                                         {"iso", ((DateTime) value).ToUniversalTime().ToString("o")}
                                      });
      }
      
      public override object ReadJson(JsonReader reader, Type type, object existingValue, JsonSerializer serializer)
      {
         return serializer.Deserialize(reader, type);
      }
   }
}
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Parse
{
   public class GeoPointConverter : JsonConverter
   {
      public override bool CanConvert(Type type)
      {
         return (typeof(GeoPoint).IsAssignableFrom(type));
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
         var geoPoint = (GeoPoint) value;
         serializer.Serialize(writer, new Dictionary<string, object>
                                      {
                                         {"__type", "GeoPoint"},
                                         {"latitude", geoPoint.Latitude},
                                         {"longitude", geoPoint.Longitude},
                                      });
      }

      public override object ReadJson(JsonReader reader, Type type, object existingValue, JsonSerializer serializer)
      {
         return serializer.Deserialize(reader, type);
      }
   }
}
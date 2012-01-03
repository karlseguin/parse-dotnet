using System;
using Newtonsoft.Json;

namespace Parse
{
   public interface IParseObject
   {
      string Id { get; set; }
      DateTime? CreatedAt { get; set; }
      DateTime? UpdatedAt { get; set; }
   }

   public class ParseObject : IParseObject
   {
      [JsonProperty("objectId")]
      public string Id { get; set; }
      [JsonProperty("createdAt")]
      public DateTime? CreatedAt { get; set; }
      [JsonProperty("updatedAt")]
      public DateTime? UpdatedAt { get; set; }
   }
}
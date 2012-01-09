using System;
using Newtonsoft.Json;

namespace Parse
{
   public interface IUsers
   {
      void Register(IParseUser user);
      void Register(IParseUser user, Action<Response<ParseObject>> callback);
   }

   public class Users : IUsers
   {
      public static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
                                                                    {
                                                                       ContractResolver = new ParseUserContract(),
                                                                       Converters = Driver.SerializationConverters
                                                                    };
      public void Register(IParseUser user)
      {
         Register(user, null);
      }

      public void Register(IParseUser user, Action<Response<ParseObject>> callback)
      {
         var payload = JsonConvert.SerializeObject(user, Formatting.None, _jsonSettings);
         Communicator.SendDataPayload<ParseObject>(Communicator.Post, "users", payload, r =>
         {
            if (callback == null) return;
            if (r.Success) { r.Data = JsonConvert.DeserializeObject<ParseObject>(r.Raw); }
            callback(r);
         });
      }
   }
}
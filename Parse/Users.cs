using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Parse
{
   public interface IUsers
   {
      void Register(IParseUser user);
      void Register(IParseUser user, Action<Response<ParseObject>> callback);
      void Login<T>(string username, string password, Action<Response<T>> callback) where T : IParseUser;
   }

   public class Users : IUsers
   {
      private const string _endPoint = "users";
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
         Communicator.SendDataPayload<ParseObject>(Communicator.Post, _endPoint, payload, r =>
         {
            if (callback == null) return;
            if (r.Success) { r.Data = JsonConvert.DeserializeObject<ParseObject>(r.Raw); }
            callback(r);
         });
      }

      public void Login<T>(string username, string password, Action<Response<T>> callback) where T : IParseUser
      {
         var payload = new Dictionary<string, object> { {"username", username}, {"password", password}};
         Communicator.SendQueryPayload<T>(Communicator.Get, "login", payload, r =>
         {
            if (callback == null) return;
            if (r.Success) { r.Data = JsonConvert.DeserializeObject<T>(r.Raw); }
            callback(r);
         });
      }
   }
}
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Parse.Queries;

namespace Parse
{
   public interface IObjects
   {
      void Save(object o);
      void Save(object o, Action<Response<ParseObject>> callback);

      void Update(string id, object o);
      void Update(IParseObject o);
      void Update(string id, object o, Action<Response<DateTime>> callback);
      void Update(IParseObject o, Action<Response<DateTime>> callback);
      IUpdateQuery<T> Update<T>(string id);
      IUpdateQuery<T> Update<T>(T o) where T : IParseObject;

      void Increment<T>(string id, string field, int value);
      void Increment<T>(string id, Expression<Func<T, object>> expression, int value);
      void Increment<T>(T o, Expression<Func<T, object>> expression, int value) where T : IParseObject;
      void Increment<T>(string id, string field, int value, Action<Response<int>> callback);
      void Increment<T>(string id, Expression<Func<T, object>> expression, int value, Action<Response<int>> callback);
      void Increment<T>(T o, Expression<Func<T, object>> expression, int value, Action<Response<int>> callback) where T : IParseObject;

      void Get<T>(string id, Action<Response<T>> callback);
      void Delete<T>(T o) where T : IParseObject;
      void Delete<T>(string id, Action<Response<T>> callback);
      void Delete<T>(T o, Action<Response<T>> callback) where T : IParseObject;
      IParseQuery<T> Query<T>();
   }

   public class Objects : IObjects
   {
      public void Save(object o)
      {
         Save(o, null);
      }

      public void Save(object o, Action<Response<ParseObject>> callback)
      {
         var payload = JsonConvert.SerializeObject(o, Driver.SerializationConverters);
         Communicator.SendDataPayload<ParseObject>(Communicator.Post, UrlFor(o), payload, r =>
         {
            if (callback == null) return;
            if (r.Success) { r.Data = JsonConvert.DeserializeObject<ParseObject>(r.Raw);}
            callback(r);
         });
      }

      public void Update(string id, object o)
      {
         Update(id, o, null);
      }

      public void Update(IParseObject o)
      {
         Update(o, null);
      }

      public void Update(IParseObject o, Action<Response<DateTime>> callback)
      {
         Update(o.Id, o, callback);
      }

      public IUpdateQuery<T> Update<T>(string id)
      {
         return new UpdateQuery<T>(this, id);
      }

      public IUpdateQuery<T> Update<T>(T o) where T : IParseObject
      {
         return Update<T>(o.Id);
      }

      public void Update(string id, object o, Action<Response<DateTime>> callback)
      {
         var url = string.Concat(UrlFor(o), "/", id);
         var payload = JsonConvert.SerializeObject(o, Driver.SerializationConverters);
         DoUpdate(url, payload, callback);
      }

      public void Update<T>(string id, IDictionary<string, object> document, Action<Response<DateTime>> callback)
      {
         var url = string.Concat(UrlFor<T>(), "/", id);
         var payload = JsonConvert.SerializeObject(document, Driver.SerializationConverters);
         DoUpdate(url, payload, callback);
      }

      public void Increment<T>(T o, Expression<Func<T, object>> expression, int value) where T : IParseObject
      {
         Increment(o.Id, expression, value, null);
      }

      public void Increment<T>(string id, Expression<Func<T, object>> expression, int value)
      {
         Increment<T>(id, expression.Body.GetMemberName(), value, null);
      }
      public void Increment<T>(string id, string field, int value)
      {
         Increment<T>(id, field, value, null);
      }
      public void Increment<T>(T o, Expression<Func<T, object>> expression, int value, Action<Response<int>> callback) where T : IParseObject
      {
         Increment(o.Id, expression, value, callback);
      }

      public void Increment<T>(string id, Expression<Func<T, object>> expression, int value, Action<Response<int>> callback)
      {
         Increment<T>(id, expression.Body.GetMemberName(), value, callback);
      }
      public void Increment<T>(string id, string field, int value, Action<Response<int>> callback)
      {
         var url = string.Concat(UrlFor<T>(), "/", id);
         var payload = new Dictionary<string, IDictionary<string, object>>
                       {
                          {field, new Dictionary<string, object> {{"__op", "Increment"}, {"amount", value}}}
                       };
         Communicator.SendDataPayload<int>(Communicator.Put, url, JsonConvert.SerializeObject(payload), r =>
         {
            if (callback == null) { return; }
            if (r.Success)
            {
               var o = JObject.Parse(r.Raw);
               for (var current = o.First; current != null; current = current.Next)
               {
                  var property = current as JProperty;
                  if (property != null && string.Compare(property.Name, "updatedAt", StringComparison.InvariantCultureIgnoreCase) != 0)
                  {
                     r.Data = property.Value.Value<int>();
                     break;
                  }
               }
            }
            callback(r);
         });
      }



      public void Get<T>(string id, Action<Response<T>> callback)
      {
         var url = string.Concat(UrlFor<T>(), "/", id);
         Communicator.SendQueryPayload(Communicator.Get, url, GotInstanceCallback(callback));
      }

      public void Delete<T>(T o) where T : IParseObject
      {
         Delete(o, null);
      }

      public void Delete<T>(T o, Action<Response<T>> callback) where T : IParseObject
      {
         Delete(o.Id, callback);
      }

      public void Delete<T>(string id, Action<Response<T>> callback)
      {
         var url = string.Concat(UrlFor<T>(), "/", id);
         Communicator.SendQueryPayload(Communicator.Delete, url, GotInstanceCallback(callback));
      }

      public IParseQuery<T> Query<T>()
      {
         return new ParseQuery<T>(this);
      }

      public void Query<T>(IDictionary<string, object> selector, Action<Response<ResultsResponse<T>>> callback)
      {
         Communicator.SendQueryPayload<ResultsResponse<T>>(Communicator.Get, UrlFor<T>(), selector, r =>
         {
            if (r.Success) { r.Data = JsonConvert.DeserializeObject<ResultsResponse<T>>(r.Raw); }
            callback(r);
         });
      }

      private static void DoUpdate(string url, string payload, Action<Response<DateTime>> callback)
      {
         Communicator.SendDataPayload<DateTime>(Communicator.Put, url, payload, r =>
         {
            if (callback == null) return;
            if (r.Success) { r.Data = JsonConvert.DeserializeObject<UpdatedAtContainer>(r.Raw).UpdatedAt; }
            callback(r);
         });
      }

      private static Action<Response<T>> GotInstanceCallback<T>(Action<Response<T>> callback)
      {
         return r =>
         {
            if (callback == null) return;
            if (r.Success) { r.Data = JsonConvert.DeserializeObject<T>(r.Raw); }
            callback(r);
         };
      }

      private static string UrlFor<T>()
      {
         return UrlFor(typeof (T));
      }
      private static string UrlFor(object o)
      {
         return UrlFor(o.GetType());
      }
      private static string UrlFor(Type t)
      {
         return string.Concat("classes/", t.Name);
      }

      private class UpdatedAtContainer
      {
         [JsonProperty("updatedAt")]
         public DateTime UpdatedAt { get; set; }
      }
   }
}
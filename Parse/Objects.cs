using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Parse.Queries;

namespace Parse
{
   public interface IObjects
   {
      void Save(object o);
      void Save(object o, Action<Response<ParseObject>> callback);
      void Update(string id, object ok);
      void Update(IParseObject o);
      void Update(string id, object o, Action<Response<DateTime>> callback);
      void Update(IParseObject o, Action<Response<DateTime>> callback);
      void Get<T>(string id, Action<Response<T>> callback);
      void Delete<T>(T o) where T : IParseObject;
      void Delete<T>(string id, Action<Response<T>> callback);
      void Delete<T>(T o, Action<Response<T>> callback) where T : IParseObject;
      IParseQuery<T> Query<T>();
   }

   public class Objects : IObjects
   {
      private static readonly JsonConverter[] _serializationConverters = new[] {(JsonConverter)new DateConverter(), new ByteConverterr()};
      public void Save(object o)
      {
         Save(o, null);
      }

      public void Save(object o, Action<Response<ParseObject>> callback)
      {
         var payload = JsonConvert.SerializeObject(o, _serializationConverters);
         Communicator.SendDataPayload<ParseObject>(Communicator.Post, UrlFor(o), payload, r =>
         {
            if (callback == null) return;
            if (r.Success) { r.Data = JsonConvert.DeserializeObject<ParseObject>(r.Raw);}
            callback(r);
         });
      }

      public void Update(string id, object ok)
      {
         Update(id, ok, null);
      }

      public void Update(IParseObject o)
      {
         Update(o, null);
      }

      public void Update(IParseObject o, Action<Response<DateTime>> callback)
      {
         Update(o.Id, o, callback);
      }

      public void Update(string id, object o, Action<Response<DateTime>> callback)
      {
         var url = string.Concat(UrlFor(o), "/", id);
         var payload = JsonConvert.SerializeObject(o, _serializationConverters);
         Communicator.SendDataPayload<DateTime>(Communicator.Put, url, payload, r =>
         {
            if (callback == null) return;
            if (r.Success) { r.Data = JsonConvert.DeserializeObject<UpdatedAtContainer>(r.Raw).UpdatedAt; }
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

      public void Query<T>(IDictionary<string, object> selector, Action<Response<ResultsContainer<T>>> callback)
      {
         Communicator.SendQueryPayload<ResultsContainer<T>>(Communicator.Get, UrlFor<T>(), selector, r =>
         {
            if (r.Success) { r.Data = JsonConvert.DeserializeObject<ResultsContainer<T>>(r.Raw); }
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
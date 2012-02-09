using System;
using System.Text;
using Newtonsoft.Json;

namespace Parse
{
   public interface IFiles
   {
      void Save(string name, string data, Action<Response<FileResponse>> callback);
      void Save(string name, byte[] data, string contentType, Action<Response<FileResponse>> callback);
      void Delete(string name, Action<Response> callback);
   }

   public class Files : IFiles
   {
      public void Save(string name, string data, Action<Response<FileResponse>> callback)
      {
         Save(name, Encoding.UTF8.GetBytes(data), "text/plain", callback);
      }

      public void Save(string name, byte[] data, string contentType, Action<Response<FileResponse>> callback)
      {
         Communicator.SendDataPayload<FileResponse>(Communicator.Post, UrlFor(name), data, contentType, r =>
         {
            if (callback == null) return;
            if (r.Success) { r.Data = JsonConvert.DeserializeObject<FileResponse>(r.Raw); }
            callback(r);
         });
      }

      public void Delete(string name, Action<Response> callback)
      {
         Communicator.SendQueryPayload<FileResponse>(Communicator.Delete, UrlFor(name), true, r =>
         {
            if (callback != null) { callback(r); }
         });
      }

      protected string UrlFor(string name)
      {
         return string.Concat("files/", name);
      }
   }
}
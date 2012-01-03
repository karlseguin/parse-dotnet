using System;
using Newtonsoft.Json;

namespace Parse
{
   public class Response
   {
      public bool Success { get; internal set; }
      internal string Raw { get; set; }
      public ErrorMessage Error { get; internal set; }
   }
   public class Response<T> : Response
   {      
      public T Data { get; internal set; }

      public static Response<T> CreateSuccess(string raw)
      {
         return new Response<T> { Success = true, Raw = raw };
      }
      public static Response<T> CreateError(ErrorMessage error)
      {
         return new Response<T> { Success = false, Error = error };
      }      
   }

   public class ErrorMessage
   {
      [JsonProperty("error")]
      public string Message { get; internal set; }
      public int Code { get; internal set; }
      public Exception InnerException { get; internal set; }
   }
}
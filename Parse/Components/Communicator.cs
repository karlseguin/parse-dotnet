using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Parse
{
   internal static class Communicator
   {
      public const string Get = "GET";
      public const string Put = "PUT";
      public const string Post = "POST";
      public const string Delete = "DELETE";

      private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore, };

      public static void SendQueryPayload<T>(string method, string endPoint, Action<Response<T>> callback)
      {
         SendQueryPayload(method, endPoint, null, false, callback);
      }

      public static void SendQueryPayload<T>(string method, string endPoint, bool includeMasterKey, Action<Response<T>> callback)
      {
         SendQueryPayload(method, endPoint, null, includeMasterKey, callback);
      }

      public static void SendQueryPayload<T>(string method, string endPoint, IDictionary<string, object> payload, Action<Response<T>> callback)
      {
         SendQueryPayload(method, endPoint, payload == null ? null : DictionaryToQueryString(payload), callback);
      }

      public static void SendQueryPayload<T>(string method, string endPoint, string query, Action<Response<T>> callback)
      {
         SendQueryPayload(method, endPoint, query, false, callback);
      }

      public static void SendQueryPayload<T>(string method, string endPoint, string query, bool includeMasterKey, Action<Response<T>> callback)
      {
         var request = BuildRequest(method, endPoint, query, callback);
         if (request != null)
         {
            if (includeMasterKey)
            {
               request.Headers["X-Parse-Master-Key"] = ParseConfiguration.Configuration.MasterKey;
            }
            request.BeginGetResponse(GetResponseStream<T>, new RequestState<T> { Request = request, Callback = callback });
         }
      }

      public static void SendDataPayload<T>(string method, string endPoint, string payload, Action<Response<T>> callback)
      {
         SendDataPayload(method, endPoint, payload, "application/json", callback);
      }

      public static void SendDataPayload<T>(string method, string endPoint, string payload, string contentType, Action<Response<T>> callback)
      {
         SendDataPayload(method, endPoint, Encoding.UTF8.GetBytes(payload), contentType, callback);
      }

      public static void SendDataPayload<T>(string method, string endPoint, byte[] payload, string contentType, Action<Response<T>> callback)
      {
         var request = BuildRequest(method, endPoint, null, callback);
         if (request != null)
         {
            request.ContentType = contentType;
            request.BeginGetRequestStream(GetRequestStream<T>, new RequestState<T> { Request = request, Payload = payload, Callback = callback });
         }
      }

      private static HttpWebRequest BuildRequest<T>(string method, string endPoint, string queryString, Action<Response<T>> callback)
      {
         var configuration = ParseConfiguration.Configuration;
         if (!configuration.NetworkCheck())
         {
            if (callback != null) { callback(Response<T>.CreateError(new ErrorMessage { Message = "Network is not available" })); }
            return null;
         }
         var url = string.Concat(configuration.Url, ParseConfiguration.ApiVersion, "/", endPoint);
         if (queryString != null)
         {
            url += string.Concat('?', queryString);
         }
         var request = (HttpWebRequest)WebRequest.Create(url);
         request.Method = method;
         request.UserAgent = "parse-dotnet";
         request.Headers["X-Parse-Application-Id"] = configuration.ApplicationId;
         request.Headers["X-Parse-REST-API-Key"] = configuration.RestApiKey;
         return request;
      }

      private static void GetRequestStream<T>(IAsyncResult result)
      {
         var state = (RequestState<T>)result.AsyncState;
         using (var requestStream = state.Request.EndGetRequestStream(result))
         {
            requestStream.Write(state.Payload, 0, state.Payload.Length);
            requestStream.Flush();
            requestStream.Close();
         }
         state.Request.BeginGetResponse(GetResponseStream<T>, state);
      }

      private static void GetResponseStream<T>(IAsyncResult result)
      {
         var state = (ResponseState<T>)result.AsyncState;
         try
         {
            using (var response = (HttpWebResponse)state.Request.EndGetResponse(result))
            {
               if (state.Callback != null) { state.Callback(Response<T>.CreateSuccess(GetResponseBody(response))); }
            }
         }
         catch (Exception ex)
         {
            if (state.Callback != null) { state.Callback(Response<T>.CreateError(HandleException(ex))); }
         }
      }

      private static string DictionaryToQueryString(IEnumerable<KeyValuePair<string, object>> payload)
      {
         var sb = new StringBuilder();
         foreach (var kvp in payload)
         {
            if (kvp.Value == null) { continue; }
            var valueType = kvp.Value.GetType();
            if (!typeof(string).IsAssignableFrom(valueType) && typeof(IEnumerable).IsAssignableFrom(valueType))
            {
               sb.Append(Serialize(kvp.Key, (IEnumerable) kvp.Value));
            }
            else
            {
               sb.Append(SerializeSingleParameter(kvp.Key, kvp.Value.ToString()));
            }
         }
         return sb.Remove(sb.Length - 1, 1).ToString();
      }

      private static string Serialize(string key, IEnumerable values)
      {
         var sb = new StringBuilder();
         foreach(var value in values)
         {
            sb.Append(SerializeSingleParameter(key, value.ToString()));
         }
         return sb.ToString();
      }

      private static string SerializeSingleParameter(string key, string value)
      {
         return string.Concat(key, '=', Uri.EscapeDataString(value), '&');
      }

      private static string GetResponseBody(WebResponse response)
      {
         using (var stream = response.GetResponseStream())
         {
            var sb = new StringBuilder();
            int read;
            var bufferSize = response.ContentLength == -1 ? 2048 : (int)response.ContentLength;
            if (bufferSize == 0) { return null; }
            do
            {
               var buffer = new byte[2048];
               read = stream.Read(buffer, 0, buffer.Length);
               sb.Append(Encoding.UTF8.GetString(buffer, 0, read));
            } while (read > 0);
            return sb.ToString();
         }
      }

      private static ErrorMessage HandleException(Exception exception)
      {
         if (exception is WebException)
         {
            var response = ((WebException) exception).Response;
            if (response == null)
            {
               return new ErrorMessage {Message = "Null response (wakeup from rehydrating (multitasking)?)", InnerException = exception};
            }
            var body = GetResponseBody(response);
            try
            {
               var message = JsonConvert.DeserializeObject<ErrorMessage>(body, _jsonSettings);
               message.InnerException = exception;
               return message;
            }
            catch (Exception)
            {
               return new ErrorMessage { Message = body, InnerException = exception };
            }
         }
         return new ErrorMessage { Message = "Unknown Error", InnerException = exception };
      }

      private class ResponseState<T>
      {
         public HttpWebRequest Request { get; set; }
         public Action<Response<T>> Callback { get; set; }
      }

      private class RequestState<T> : ResponseState<T> 
      {         
         public byte[] Payload { get; set; }
      }
   }
}
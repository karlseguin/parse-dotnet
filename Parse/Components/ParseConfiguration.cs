using System;
using System.Net.NetworkInformation;

namespace Parse
{
   public interface IParseConfiguration
   {
      IParseConfiguration ConnectTo(string url);
      IParseConfiguration NetworkAvailableCheck(Func<bool> networkCheck);
   }

   public class ParseConfiguration : IParseConfiguration
   {
      public const string ApiVersion = "1";
      private static readonly ParseConfiguration _configuration = new ParseConfiguration();
      public string Url { get; private set; }
      public Func<bool> NetworkCheck { get; private set; }
      public string ApplicationId { get; private set; }
      public string RestApiKey { get; private set; }
      public string MasterKey { get; private set; }
      public static ParseConfiguration Configuration
      {
         get { return _configuration; }
      }

      protected ParseConfiguration()
      {
      }

      public IParseConfiguration ConnectTo(string url)
      {
         _configuration.Url = url;
         return this;
      }

      public IParseConfiguration NetworkAvailableCheck(Func<bool> networkCheck)
      {
         _configuration.NetworkCheck = networkCheck;
         return this;
      }

      public static void Configure(string applicationId, string restApiKey)
      {
         Configure(applicationId, restApiKey, null, null);
      }

      public static void Configure(string applicationId, string restApiKey, string masterKey)
      {
         Configure(applicationId, restApiKey, masterKey, null);
      }

      public static void Configure(string applicationId, string restApiKey, Action<IParseConfiguration> action)
      {
         Configure(applicationId, restApiKey, null, action);
      }

      public static void Configure(string applicationId, string restApiKey, string masterKey, Action<IParseConfiguration> action)
      {
         _configuration.NetworkAvailableCheck(NetworkInterface.GetIsNetworkAvailable);
         _configuration.ApplicationId = applicationId;
         _configuration.RestApiKey = restApiKey;
         _configuration.MasterKey = masterKey;
         _configuration.Url = "https://api.parse.com/";
         if (action != null)
         {
            action(_configuration);
         }
      }
   }
}
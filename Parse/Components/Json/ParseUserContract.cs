using Newtonsoft.Json.Serialization;

namespace Parse
{
   internal class ParseUserContract : DefaultContractResolver
   {
      protected override string ResolvePropertyName(string propertyName)
      {
         return propertyName == "UserName" || propertyName == "Password" ? propertyName.ToLower() : propertyName;
      }
   }
}
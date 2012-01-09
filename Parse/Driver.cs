using Newtonsoft.Json;

namespace Parse
{
   public interface IDriver
   {
      IObjects Objects { get; }
      IUsers Users { get; }
   }

   public class Driver : IDriver
   {
      internal static readonly JsonConverter[] SerializationConverters = new[] { (JsonConverter)new DateConverter(), new ByteConverterr(), new GeoPointConverter() };

      private readonly IObjects _objects;
      private readonly IUsers _users;

      public IObjects Objects
      {
         get { return _objects; }
      }

      public IUsers Users
      {
         get { return _users; }
      }

      public Driver()
      {
         _objects = new Objects();
         _users = new Users();
      }
   }
}
namespace Parse
{
   public interface IDriver
   {
      IObjects Objects { get; }
   }

   public class Driver : IDriver
   {
      private readonly IObjects _objects;

      public IObjects Objects
      {
         get { return _objects; }
      }
      public Driver()
      {
         _objects = new Objects();
      }
   }
}
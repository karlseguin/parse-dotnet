using System.Collections.Generic;

namespace Parse.Tests
{
   public class ParseObjectClass : ParseObject
   {
      public string Name { get; set; }
   }

   public class ParseUser : IParseUser
   {
      public string UserName { get; set; }
      public string Password { get; set; }
      public int PowerLevel { get; set; }
   }

   public class ComplexParseObjectClass : ParseObject
   {
      public string Name { get; set; }
      public int PowerLevel { get; set; }
      public bool Sayan { get; set; }
   }

   public class ByteArrayClass
   {
      public byte[] Nibbles { get; set; }
   }

   public class ByteEnumeratorClass
   {
      public IList<byte> Nibbles { get; set; }
   }

   public class GeoPointClass
   {
      public GeoPoint Location { get; set; }
   }

   public class FileClass
   {
      public ParseFile File { get; set; }
   }

   public class SimpleClass
   {
      public int PowerLevel { get; set; }
   }
}
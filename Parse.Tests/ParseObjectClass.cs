namespace Parse.Tests
{
   public class ParseObjectClass : ParseObject
   {
      public string Name { get; set; }
   }

   public class ComplexParseObjectClass : ParseObject
   {
      public string Name { get; set; }
      public int PowerLevel { get; set; }
      public bool Sayan { get; set; }
   }


   public class SimpleClass
   {
      public int PowerLevel { get; set; }
   }
}
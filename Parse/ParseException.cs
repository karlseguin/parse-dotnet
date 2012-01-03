using System;

namespace Parse
{
   public class ParseException : Exception
   {
      public ErrorMessage Details { get; set; }

      public ParseException() { }
      public ParseException(ErrorMessage message) : this(message.Message, message.InnerException){}
      public ParseException(string message) : base(message) { }
      public ParseException(string message, Exception innerException) : base(message, innerException) { }
      public ParseException(ErrorMessage message, Exception innerException) : base(message.Message, innerException)
      {
         Details = message;
      }
   }
}
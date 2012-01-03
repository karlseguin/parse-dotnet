using System.Collections.Generic;

namespace Parse
{
   public class ResultsContainer<T>
   {
      public IList<T> Results { get; set; }
      public int Count { get; set; }
   }
}
using System.Collections.Generic;

namespace Parse
{
   public class ResultsResponse<T>
   {
      public IList<T> Results { get; set; }
      public int Count { get; set; }
   }
}
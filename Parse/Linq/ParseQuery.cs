using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Parse.Linq
{
   public class ParseQuery<T> : IQueryable<T>
   {
      private readonly IQueryProvider _provider;
      private readonly Expression _expression;

      public ParseQuery(IQueryProvider provider)
      {
         _provider = provider;
         _expression = Expression.Constant(this);
      }
      public ParseQuery(IQueryProvider provider, Expression expression)
      {
         _provider = provider;
         _expression = expression;
      }

      public Expression Expression
      {
         get { return _expression; }
      }

      public Type ElementType
      {
         get { return typeof (T); }
      }

      public IQueryProvider Provider
      {
         get { return _provider;  }
      }

      public IEnumerator<T> GetEnumerator()
      {
         return ((IEnumerable<T>)_provider.Execute(_expression)).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      public override string ToString()
      {
         return _provider.GetQueryText(_expression);
      }
   }
}
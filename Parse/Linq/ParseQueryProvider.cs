using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Parse.Linq
{
   public class ParseQueryProvider : IQueryProvider
   {
      public IQueryable CreateQuery(Expression expression)
      {
         try
         {
            return (IQueryable)Activator.CreateInstance(typeof(ParseQuery<>).MakeGenericType(TypeSystem.GetElementType(expression.Type)), new object[] { this, expression });
         }
         catch (TargetInvocationException e)
         {
            throw e.InnerException;
         }
      }

      public IQueryable<T> CreateQuery<T>(Expression expression)
      {
         return new ParseQuery<T>(this, expression);
      }

      public T Execute<T>(Expression expression)
      {
         return (T) Execute(expression);
      }

      public object Execute(Expression expression)
      {
         return Execute(expression);
      }

      public string GetQueryText(Expression expression)
      {
         
      }
   }
}
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Parse.Queries
{
   public interface IUpdateQuery<T>
   {
      IUpdateQuery<T> Set(Expression<Func<T, object>> expression, object value);
      void Execute(Action<Response<DateTime>> callback);
      void Execute();
   }

   public class UpdateQuery<T> : IUpdateQuery<T>
   {
      private readonly Objects _objects;
      private readonly string _id;
      private readonly IDictionary<string, object> _values = new Dictionary<string, object>();

      public UpdateQuery(Objects objects, string id)
      {
         _objects = objects;
         _id = id;
      }

      public IUpdateQuery<T> Set(Expression<Func<T, object>> expression, object value)
      {
         _values[expression.Body.GetMemberName()] = value;
         return this;
      }

      public void Execute()
      {
         Execute(null);
      }

      public void Execute(Action<Response<DateTime>> callback)
      {
         _objects.Update<T>(_id, _values, callback);
      }
   }
}
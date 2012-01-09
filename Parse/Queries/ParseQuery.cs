using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace Parse.Queries
{
   public interface IParseQuery<T>
   {
      IParseQuery<T> Where(Expression<Func<T, bool>> expression);
      IParseQuery<T> Count();
      IParseQuery<T> Skip(int skip);
      IParseQuery<T> Limit(int limit);
      IParseQuery<T> Sort(Expression<Func<T, object>> expression, bool ascending);
      IParseQuery<T> SortAscending(Expression<Func<T, object>> expression);
      IParseQuery<T> SortDescending(Expression<Func<T, object>> expression);
      void Execute(Action<Response<ResultsResponse<T>>> callback);
   }

   public class ParseQuery<T> : ExpressionVisitor, IParseQuery<T>
   {
      private readonly Objects _objects;
      private IDictionary<string, object> _where;
      private bool _count;
      private int? _skip;
      private int? _limit;
      private bool _sortDirection;
      private string _sort;

      public ParseQuery(Objects objects)
      {
         _objects = objects;
      }

      public IParseQuery<T> Where(Expression<Func<T, bool>> expression)
      {
         _where = new WhereTranslator().Translate(expression.Body);
         return this;
      }

      public IParseQuery<T> Count()
      {
         _count = true;
         return this;
      }

      public IParseQuery<T> Skip(int skip)
      {
         _skip = skip;
         return this;
      }

      public IParseQuery<T> Limit(int limit)
      {
         _limit = limit;
         return this;
      }

      public IParseQuery<T> SortAscending(Expression<Func<T, object>> expression)
      {
         return Sort(expression, true);
      }

      public IParseQuery<T> SortDescending(Expression<Func<T, object>> expression)
      {
         return Sort(expression, false);
      }

      public IParseQuery<T> Sort(Expression<Func<T, object>> expression, bool ascending)
      {
         var name = expression.Body.GetMemberName();
         if (name == null)
         {
            throw new NotSupportedException(string.Format("The member '{0}' is not supported for sorting", expression.Body.NodeType));
         }
         _sort = name;
         _sortDirection = ascending;
         return this;
      }

      public void Execute(Action<Response<ResultsResponse<T>>> callback)
      {
         var dictionary = new Dictionary<string, object>();
         if (_where != null) { dictionary["where"] = JsonConvert.SerializeObject(_where); }
         if (_count) { dictionary["count"] = '1'; }
         if (_skip.HasValue) { dictionary["skip"] = _skip.Value; }
         if (_limit.HasValue) { dictionary["limit"] = _limit.Value; }
         if (_sort != null) { dictionary["order"] = _sortDirection ? _sort : string.Concat('-', _sort); }
         _objects.Query(dictionary, callback);
      }
   }
}
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Parse.Queries
{
   internal class WhereTranslator : ExpressionVisitor
   {
      private IDictionary<string, object> _where;
      private string _currentKey;
      private string _currentOperation;
      private bool _inversed;
      

      internal IDictionary<string, object> Translate(Expression expression)
      {
         _where = new Dictionary<string, object>();
         Visit(expression);
         return _where;
      }

      protected override Expression VisitUnary(UnaryExpression u)
      {
         switch (u.NodeType)
         {
            case ExpressionType.Not:
               _inversed = !_inversed;
               Visit(u.Operand);
               return u;
            default:
               throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
         }
      }

      protected override Expression VisitBinary(BinaryExpression b)
      {
         _currentKey = _currentOperation = null;
         _inversed = false;
         Visit(b.Left);
         switch (b.NodeType)
         {
            case ExpressionType.Equal:
               break;
            case ExpressionType.NotEqual:
               SetNestedDictionary("$ne");
               break;
            case ExpressionType.LessThan:
               SetNestedDictionary("$lt");
               break;
            case ExpressionType.LessThanOrEqual:
               SetNestedDictionary("$lte");
               break;
            case ExpressionType.GreaterThan:
               SetNestedDictionary("$gt");
               break;
            case ExpressionType.GreaterThanOrEqual:
               SetNestedDictionary("$gte");
               break;
            case ExpressionType.AndAlso:
               break;
            default:
               throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
         }
         Visit(b.Right);
         return b;
      }

      protected override Expression VisitConstant(ConstantExpression c)
      {
         SetValue(c.Value);
         return c;
      }

      protected override Expression VisitMemberAccess(MemberExpression m)
      {
         if (m.Expression != null)
         {
            if (m.Expression.NodeType == ExpressionType.Constant)
            {
               var objectMember = Expression.Convert(m, typeof(object));
               var getterLambda = Expression.Lambda<Func<object>>(objectMember, (ParameterExpression) m.Expression);
               var getter = getterLambda.Compile();
               SetValue(getter());
               return m;
            }
            if (m.Expression.NodeType == ExpressionType.Parameter)
            {
               _currentKey = m.Member.Name;
               if (!_where.ContainsKey(_currentKey))
               {
                  if (m.Member is PropertyInfo && ((PropertyInfo)m.Member).PropertyType == typeof(bool))
                  {
                     _where[_currentKey] = !_inversed;
                  }
                  else
                  {
                     _where[_currentKey] = null;
                  }
                  
               }
               return m;
            }
         }
         throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
      }

      private void SetNestedDictionary(string operation)
      {
         var nested = _where[_currentKey] as Dictionary<string, object>;
         if (nested == null)
         {
            nested = new Dictionary<string, object>(3);
            _where[_currentKey] = nested;
         }
         _currentOperation = operation;
         nested.Add(operation, null);
      }

      private void SetValue(object o)
      {
         if (string.IsNullOrEmpty(_currentKey))
         {
            throw new InvalidOperationException("Value can only be on the right-hand side of an operation");
         }
         if (_currentOperation == null)
         {
            _where[_currentKey] = o;
         }
         else
         {
            ((IDictionary<string, object>)_where[_currentKey])[_currentOperation] = o;
         }
      }
   }
}
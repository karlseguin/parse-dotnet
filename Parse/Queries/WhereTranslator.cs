using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Parse.Queries
{
   internal class WhereTranslator : ExpressionVisitor
   {
      private const string _regexOperation = "$regex";
      private string _currentKey;
      private string _currentOperation;
      private bool _inversed;
      private IDictionary<string, object> _where;

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
         ResetContext();
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

      private void ResetContext()
      {
         _currentKey = _currentOperation = null;
         _inversed = false;
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
               SetValue(m.GetValue());
               return m;
            }
            if (m.Expression.NodeType == ExpressionType.Parameter)
            {
               _currentKey = m.Member.Name;

               // Use JsonProperty if it exists..
               var jsonName = (Newtonsoft.Json.JsonPropertyAttribute)m.Member.GetCustomAttributes(false).FirstOrDefault(x => x is Newtonsoft.Json.JsonPropertyAttribute);
               if (jsonName != null)
               {
                   _currentKey = jsonName.PropertyName;
               }

               if (!_where.ContainsKey(_currentKey))
               {
                  if (m.Member is PropertyInfo && ((PropertyInfo) m.Member).PropertyType == typeof (bool))
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

      protected override Expression VisitMethodCall(MethodCallExpression m)
      {
         if (m.Method.DeclaringType == typeof (Regex) && m.Method.Name == "IsMatch")
         {
            HandleRegexIsMatch(m);
            return m;
         }
         if (m.Method.DeclaringType == typeof (Enumerable) && m.Method.Name == "Contains")
         {
            HandleContains(m);
            return m;
         }
         if (m.Method.DeclaringType == typeof (string))
         {
            switch (m.Method.Name)
            {
               case "StartsWith":
               {
                  return HandleRegexIsMatch(m, "^{0}");
               }
               case "EndsWith":
               {
                  return HandleRegexIsMatch(m, "{0}$");
               }
               case "Contains":
               {
                  return HandleRegexIsMatch(m, "{0}");
               }
            }
         }
         if (m.Method.DeclaringType == typeof (GeoPoint) && m.Method.Name == "NearSphere")
         {
            HandleNearSphere(m);
            return m;
         }
         throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
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
            ((IDictionary<string, object>) _where[_currentKey])[_currentOperation] = o;
         }
      }

      private void HandleContains(MethodCallExpression m)
      {
         ResetContext();
         Visit(m.Arguments[1]);
         SetNestedDictionary("$in");
         Visit(m.Arguments[0]);
      }

      private void HandleRegexIsMatch(MethodCallExpression m)
      {
         ResetContext();
         Visit(m.Arguments[0]);
         SetNestedDictionary(_regexOperation);
         Visit(m.Arguments[1]);
      }

      private void HandleNearSphere(MethodCallExpression m)
      {
         ResetContext();
         var data = new Dictionary<string, object>
                    {
                       {
                          "$nearSphere", new Dictionary<string, object>
                                         {
                                            {"__type", "GeoPoint"},
                                            {"latitude", m.Arguments[0].GetValue()},
                                            {"longitude", m.Arguments[1].GetValue()},
                                         }
                          }
                    };

         if (m.Arguments.Count == 3)
         {
            data["$maxDistanceInMiles"] = m.Arguments[2].GetValue();
         }
         _where[m.Object.GetMemberName()] = data;

      }

      private Expression HandleRegexIsMatch(MethodCallExpression m, string pattern)
      {
         ResetContext();
         Visit(m.Object);
         SetNestedDictionary(_regexOperation);
         Visit(m.Arguments[0]);
         var value = (IDictionary<string, object>) _where[_currentKey];
         value[_regexOperation] = string.Format(pattern, value[_regexOperation]);
         return m;
      }
   }
}
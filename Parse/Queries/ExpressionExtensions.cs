using System;
using System.Linq.Expressions;

namespace Parse.Queries
{
   public static class ExpressionExtensions
   {
      public static string GetMemberName(this Expression expression)
      {
         if (expression is UnaryExpression)
         {
            expression = ((UnaryExpression) expression).Operand;
         }
         if (expression is MemberExpression)
         {
            return ((MemberExpression) expression).Member.Name;
         }
         return null;
      }

      public static object GetValue(this Expression expression)
      {
         if (expression is ConstantExpression)
         {
            return ((ConstantExpression) expression).Value;
         }
         if (expression is MemberExpression)
         {
            var m = (MemberExpression)expression;
            if (m.Expression.NodeType == ExpressionType.Constant)
            {
               var objectMember = Expression.Convert(m, typeof (object));
               var getterLambda = Expression.Lambda<Func<object>>(objectMember);
               return getterLambda.Compile()();
            }
         }
         if (expression is UnaryExpression)
         {
            return ((UnaryExpression) expression).Operand.GetValue();
         }
         return null;
      }
   }
}
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
   }
}
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Parse.Linq
{
   internal class QueryTranslator : ExpressionVisitor
   {
      private StringBuilder _builder;

      internal string Translate(Expression expression)
      {
         _builder = new StringBuilder();
         Visit(expression);
         return _builder.ToString();
      }

      private static Expression StripQuotes(Expression e)
      {
         while (e.NodeType == ExpressionType.Quote)
         {
            e = ((UnaryExpression) e).Operand;
         }
         return e;
      }

      protected override Expression VisitMethodCall(MethodCallExpression m)
      {
         if (m.Method.DeclaringType == typeof (Queryable) && m.Method.Name == "Where")
         {
            _builder.Append("SELECT * FROM (");
            Visit(m.Arguments[0]);
            _builder.Append(") AS T WHERE ");
            var lambda = (LambdaExpression) StripQuotes(m.Arguments[1]);
            Visit(lambda.Body);
            return m;
         }
         throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
      }

      protected override Expression VisitUnary(UnaryExpression u)
      {
         switch (u.NodeType)
         {
            case ExpressionType.Not:
               _builder.Append(" NOT ");
               Visit(u.Operand);
               break;
            default:
               throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
         }
         return u;
      }

      protected override Expression VisitBinary(BinaryExpression b)
      {
         _builder.Append("(");
         Visit(b.Left);
         switch (b.NodeType)
         {
            case ExpressionType.And:
               _builder.Append(" AND ");
               break;
            case ExpressionType.Or:
               _builder.Append(" OR");
               break;
            case ExpressionType.Equal:
               _builder.Append(" = ");
               break;
            case ExpressionType.NotEqual:
               _builder.Append(" <> ");
               break;
            case ExpressionType.LessThan:
               _builder.Append(" < ");
               break;
            case ExpressionType.LessThanOrEqual:
               _builder.Append(" <= ");
               break;
            case ExpressionType.GreaterThan:
               _builder.Append(" > ");
               break;
            case ExpressionType.GreaterThanOrEqual:
               _builder.Append(" >= ");
               break;
            default:
               throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
         }
         Visit(b.Right);
         _builder.Append(")");
         return b;
      }

      protected override Expression VisitConstant(ConstantExpression c)
      {
         var q = c.Value as IQueryable;
         if (q != null)
         {
            // assume constant nodes w/ IQueryables are table references
            _builder.Append("SELECT * FROM ");
            _builder.Append(q.ElementType.Name);
         }
         else if (c.Value == null)
         {
            _builder.Append("NULL");
         }
         else
         {
            switch (Type.GetTypeCode(c.Value.GetType()))
            {
               case TypeCode.Boolean:
                  _builder.Append(((bool) c.Value) ? 1 : 0);
                  break;
               case TypeCode.String:
                  _builder.Append("'");
                  _builder.Append(c.Value);
                  _builder.Append("'");
                  break;
               case TypeCode.Object:
                  throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));
               default:
                  _builder.Append(c.Value);
                  break;
            }
         }
         return c;
      }

      protected override Expression VisitMemberAccess(MemberExpression m)
      {
         if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
         {
            _builder.Append(m.Member.Name);
            return m;
         }
         throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
      }
   }
}
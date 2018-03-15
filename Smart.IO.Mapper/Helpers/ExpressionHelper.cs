namespace Smart.IO.Mapper.Helpers
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    internal static class ExpressionHelper
    {
        public static string GetMemberName(Expression expr)
        {
            var mi = GetMemberInfo(expr);
            if (mi == null)
            {
                throw new ArgumentException("Expression is invalid.", nameof(expr));
            }

            return mi.Name;
        }

        public static MemberInfo GetMemberInfo(Expression expr)
        {
            while (true)
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.Convert:
                        expr = ((UnaryExpression)expr).Operand;
                        break;
                    case ExpressionType.Lambda:
                        expr = ((LambdaExpression)expr).Body;
                        break;
                    case ExpressionType.MemberAccess:
                        var memberExpression = (MemberExpression)expr;
                        if (memberExpression.Expression.NodeType != ExpressionType.Parameter &&
                            memberExpression.Expression.NodeType != ExpressionType.Convert)
                        {
                            throw new ArgumentException("Expression is invalid.", nameof(expr));
                        }

                        var member = memberExpression.Member;
                        return member;
                    default:
                        return null;
                }
            }
        }
    }
}

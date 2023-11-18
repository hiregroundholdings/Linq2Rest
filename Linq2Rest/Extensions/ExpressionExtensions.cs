using System.Linq.Expressions;

namespace LinqConvertTools.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression ReplaceMemberExpression(Type type, string memberName, string replacementMemberName, Expression expression, ParameterExpression? parameterExpression)
        {
            if (string.IsNullOrEmpty(memberName))
            {
                throw new ArgumentNullException(nameof(memberName));
            }

            if (string.IsNullOrEmpty(replacementMemberName))
            {
                throw new ArgumentNullException(nameof(replacementMemberName));
            }

            if (expression is LambdaExpression lambdaExpression)
            {
                ParameterExpression? pe = parameterExpression ?? (lambdaExpression.Parameters.Count > 0 ? Expression.Parameter(type, "x") : null);
                Expression body = ReplaceMemberExpression(type, memberName, replacementMemberName, lambdaExpression.Body, pe);
                return Expression.Lambda(body, pe);
            }
            else if (expression is MethodCallExpression methodCallExpression)
            {
                Expression @object = ReplaceMemberExpression(type, memberName, replacementMemberName, methodCallExpression.Object, parameterExpression);
                return methodCallExpression.Update(@object, methodCallExpression.Arguments);
            }
            else if (expression is BinaryExpression binaryExpression)
            {
                Expression left = ReplaceMemberExpression(type, memberName, replacementMemberName, binaryExpression.Left, parameterExpression);
                Expression right = ReplaceMemberExpression(type, memberName, replacementMemberName, binaryExpression.Right, parameterExpression);
                return binaryExpression.Update(left, binaryExpression.Conversion, right);
            }
            else if (expression is MemberExpression memberExpression)
            {
                ParameterExpression pe = parameterExpression ?? Expression.Parameter(type, "x");
                if (memberExpression.Member?.Name == memberName)
                {
                    return GetPropertyOrFieldExpression(pe, replacementMemberName);
                }
                else if (memberExpression.Member is not null)
                {
                    return GetPropertyOrFieldExpression(pe, memberExpression.Member.Name);
                }

                return memberExpression;
            }

            return expression;
        }

        public static Expression ReplaceMemberExpression<T>(this Expression expression, string memberName, string replacementMemberName)
        {
            return ReplaceMemberExpression(typeof(T), memberName, replacementMemberName, expression, null);
        }

        private static Expression GetPropertyOrFieldExpression(ParameterExpression parameterExpression, string propOrFieldName)
        {
            Expression parentParameterExpression = parameterExpression;
            foreach (string memberName in propOrFieldName.Split('.'))
            {
                parentParameterExpression = Expression.PropertyOrField(parentParameterExpression, memberName);
            }

            return Expression.Lambda(parentParameterExpression, parameterExpression).Body;
        }
    }
}

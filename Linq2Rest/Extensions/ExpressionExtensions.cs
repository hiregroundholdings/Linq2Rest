using System.Linq.Expressions;
using System.Reflection;

namespace LinqConvertTools.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression CastParameter<T>(this Expression expression, ParameterExpression? parameterExpression)
        {
            if (expression is LambdaExpression lambdaExpression)
            {
                ParameterExpression? pe = parameterExpression ?? (lambdaExpression.Parameters.Count > 0 ? Expression.Parameter(typeof(T), "x") : null);
                Expression body = CastParameter<T>(lambdaExpression.Body, pe);
                return Expression.Lambda(body, pe);
            }
            else if (expression is MethodCallExpression methodCallExpression)
            {
                Expression[] args = methodCallExpression.Arguments.Select(a => CastParameter<T>(a, parameterExpression)).ToArray();
                if (methodCallExpression.Object is null)
                {
                    return methodCallExpression.Update(null, args);
                }
                else
                {
                    Expression @object = CastParameter<T>(methodCallExpression.Object, parameterExpression);
                    return methodCallExpression.Update(@object, args);
                }
            }
            else if (expression is BinaryExpression binaryExpression)
            {
                Expression left = CastParameter<T>(binaryExpression.Left, parameterExpression);
                Expression right = CastParameter<T>(binaryExpression.Right, parameterExpression);

                if (left.NodeType == ExpressionType.MemberAccess && right is UnaryExpression rightUnaryExpression && rightUnaryExpression.NodeType == ExpressionType.Convert)
                {
                    right = Expression.Convert(rightUnaryExpression.Operand, left.Type);
                }
                else if (right.NodeType == ExpressionType.MemberAccess && left is UnaryExpression leftUnaryExpression && leftUnaryExpression.NodeType == ExpressionType.Convert)
                {
                    left = Expression.Convert(leftUnaryExpression.Operand, right.Type);
                }

                try
                {
                    return binaryExpression.Update(left, binaryExpression.Conversion, right);
                }
                catch (InvalidOperationException) when (left.Type != right.Type)
                {
                    if (GetImplicitOperator(right.Type, left.Type) is MethodInfo opRToL)
                    {
                        var parameterType = opRToL.GetParameters()[0].ParameterType;
                        right = LiftIfNeeded(right, parameterType);
                        right = Expression.Convert(right, left.Type, opRToL);
                    }
                    else if (GetImplicitOperator(left.Type, right.Type) is MethodInfo opLToR)
                    {
                        var parameterType = opLToR.GetParameters()[0].ParameterType;
                        left = LiftIfNeeded(left, parameterType);
                        left = Expression.Convert(left, right.Type, opLToR);
                    }
                    else
                    {
                        throw;
                    }
                    
                    return binaryExpression.Update(left, binaryExpression.Conversion, right);
                }
            }
            else if (expression is MemberExpression memberExpression && memberExpression.Member is not null)
            {
                ParameterExpression pe = parameterExpression ?? Expression.Parameter(typeof(T), "x");
                return GetPropertyOrFieldExpression(pe, memberExpression.Member.Name);
            }

            return expression;
        }

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

        private static Expression LiftIfNeeded(Expression expr, Type parameterType)
        {
            // If the operator expects Nullable<T> and we have T, lift T -> Nullable<T>
            var underlying = Nullable.GetUnderlyingType(parameterType);
            if (underlying is not null && expr.Type == underlying)
            {
                return Expression.Convert(expr, parameterType); // built-in T -> Nullable<T>
            }

            return expr;
        }

        private static MethodInfo? GetImplicitOperator(Type fromType, Type toType)
        {
            static bool Match(ParameterInfo p, Type from, Type to, MethodInfo m)
            {
                // exact: TFrom -> TTo
                if (p.ParameterType == from && m.ReturnType == to)
                {
                    return true;
                }

                // lifted: TFrom? -> TTo?
                var underlyingParameterType = Nullable.GetUnderlyingType(p.ParameterType) ?? p.ParameterType;
                var underlyingReturnType = Nullable.GetUnderlyingType(m.ReturnType) ?? m.ReturnType;
                return underlyingParameterType == from && underlyingReturnType == to;
            }

            var underlyingFromType = Nullable.GetUnderlyingType(fromType) ?? fromType;
            var underlyingToType = Nullable.GetUnderlyingType(toType) ?? toType;
            foreach (var t in new[] { underlyingFromType, underlyingToType })
            {
                foreach (var m in t.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    if (m.Name != "op_Implicit")
                    {
                        continue;
                    }

                    var ps = m.GetParameters();
                    if (ps.Length != 1)
                    {
                        continue;
                    }

                    if (Match(ps[0], fromType, toType, m))
                    {
                        return m;
                    }
                }
            }

            return null;
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

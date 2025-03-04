// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnyAllMethodWriter.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the AnyAllMethodWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools.Provider.Writers
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    internal class AnyAllMethodWriter : IMethodCallWriter
    {
        public bool CanHandle(MethodCallExpression expression)
        {


            return expression.Method.Name == "Any" || expression.Method.Name == "All";
        }

        public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
        {




            var firstArg = expressionWriter(expression.Arguments[0]);
            var method = expression.Method.Name.ToLowerInvariant();
            string parameter = null;
            var lambdaParameter = expression.Arguments[1] as LambdaExpression;
            if (lambdaParameter != null)
            {
                var first = lambdaParameter.Parameters.First();
                parameter = first.Name ?? first.ToString();
            }

            var predicate = expressionWriter(expression.Arguments[1]);

            return string.Format("{0}/{1}({2}: {3})", firstArg, method, parameter, predicate);
        }
    }
}
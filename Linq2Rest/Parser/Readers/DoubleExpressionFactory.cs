// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleExpressionFactory.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DoubleExpressionFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools.Parser.Readers
{
    using System;
    using System.Globalization;
    using System.Linq.Expressions;

    internal class DoubleExpressionFactory : ValueExpressionFactoryBase<double>
    {
        public override ConstantExpression Convert(string token)
        {
            double number;
            if (double.TryParse(token.Trim('D', 'd'), NumberStyles.Any, CultureInfo.InvariantCulture, out number))
            {
                return Expression.Constant(number);
            }

            throw new FormatException("Could not read " + token + " as double.");
        }
    }
}
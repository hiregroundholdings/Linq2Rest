// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MathCeilingMethodWriter.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MathCeilingMethodWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools.Provider.Writers
{
    using System;
    using System.Linq.Expressions;

    internal class MathCeilingMethodWriter : MathMethodWriter
    {
        protected override string MethodName
        {
            get { return "ceiling"; }
        }

        public override bool CanHandle(MethodCallExpression expression)
        {


            return expression.Method.DeclaringType == typeof(Math)
                   && expression.Method.Name == "Ceiling";
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuidValueWriter.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the GuidValueWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools.Provider.Writers
{
    using System;

    internal class GuidValueWriter : ValueWriterBase<Guid>
    {
        public override string Write(object value)
        {
            return string.Format("guid'{0}'", value);
        }
    }
}
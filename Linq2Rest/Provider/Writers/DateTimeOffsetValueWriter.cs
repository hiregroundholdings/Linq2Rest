// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeOffsetValueWriter.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DateTimeOffsetValueWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools.Provider.Writers
{
    using System;
    using System.Xml;

    internal class DateTimeOffsetValueWriter : ValueWriterBase<DateTimeOffset>
    {
        public override string Write(object value)
        {
            return string.Format("datetimeoffset'{0}'", XmlConvert.ToString((DateTimeOffset)value));
        }
    }
}
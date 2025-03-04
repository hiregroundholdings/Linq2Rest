// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ByteValueWriter.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ByteValueWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools.Provider.Writers
{
    internal class ByteValueWriter : ValueWriterBase<byte>
    {
        public override string Write(object value)
        {
            var byteValue = (byte)value;

            return byteValue.ToString("X");
        }
    }
}
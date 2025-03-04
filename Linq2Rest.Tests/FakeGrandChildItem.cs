// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeGrandChildItem.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the FakeGrandChildItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools.Tests
{
    using System.Runtime.Serialization;

    [DataContract]
    public class FakeGrandChildItem
    {
        public string GrandChildStringValue { get; set; }
    }
}
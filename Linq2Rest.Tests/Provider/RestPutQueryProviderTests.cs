// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestPutQueryProviderTests.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestPutQueryProviderTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Provider
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Linq.Expressions;
	using Linq2Rest.Provider;
	using Linq2Rest.Provider.Writers;
	using Linq2Rest.Tests.Fakes;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class RestPutQueryProviderTests
	{
		private RestPutQueryProvider<FakeItem> _provider;
		private Mock<IRestClient> _mockClient;
		private Stream _inputData;

		[OneTimeSetUp]
		public void FixtureSetup()
		{
			_inputData = "[]".ToStream();
			_mockClient = new Mock<IRestClient>();
			_mockClient.SetupGet(x => x.ServiceBase).Returns(new Uri("http://localhost"));
			_mockClient.Setup(x => x.Put(It.IsAny<Uri>(), It.IsAny<Stream>())).Returns("[]".ToStream());
			var memberNameResolver = new MemberNameResolver();
			_provider = new RestPutQueryProvider<FakeItem>(_mockClient.Object, new TestSerializerFactory(memberNameResolver), new ExpressionProcessor(new ExpressionWriter(memberNameResolver, Enumerable.Empty<IValueWriter>()), memberNameResolver), memberNameResolver, Enumerable.Empty<IValueWriter>(), _inputData, typeof(FakeItem));
		}

		[Test]
		public void WhenExecutingQueryThenPutsToRestClient()
		{
			Expression<Func<FakeItem, bool>> expression = x => true;
			_provider.Execute(expression);

			_mockClient.Verify(x => x.Put(It.IsAny<Uri>(), _inputData));
		}
	}
}
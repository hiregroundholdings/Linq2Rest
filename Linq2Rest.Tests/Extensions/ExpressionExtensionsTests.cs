using LinqConvertTools.Tests.Fakes;
using LinqCovertTools;
using LinqConvertTools.Extensions;
using NUnit.Framework;
using System.Linq.Expressions;

namespace LinqConvertTools.Tests.Extensions
{
    [TestFixture]
    public class ExpressionExtensionsTests
    {
        [Test]
        public void ConvertsFilterToExpressionForInterface()
        {
            const string filter = "startswith(emailAddress, 'user@')";
            ODataExpressionConverter converter = new();

            Expression<Func<IQueryableUser, bool>> converted = converter.Convert<IQueryableUser>(filter);
            Expression<Func<User, bool>> predicate = (Expression<Func<User, bool>>)converted.ReplaceMemberExpression<User>("EmailAddress", "EmailAddress.Value");

            List<User> users = new()
            {
                new User()
                {
                    GivenName = "User",
                    FamilyName = "One",
                    EmailAddress = new EmailAddress("user@xyz.com")
                },
                new User()
                {
                    GivenName = "User",
                    FamilyName = "Two",
                    EmailAddress = new EmailAddress("nonuser@xyz.com")
                }
            };

            IQueryable<User> query = users.AsQueryable();

            query = query.Where(predicate).Cast<User>();

            Assert.AreEqual(1, query.Count());
        }
    }
}

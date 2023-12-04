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
        public void CastMember()
        {
            // Arrange
            const string filter = "hired ge datetimeoffset'2023-10-13T09:52:00'";
            ODataExpressionConverter converter = new();
            Expression<Func<IQueryableUser, bool>> expression = converter.Convert<IQueryableUser>(filter);

            // Act
            Expression<Func<User, bool>> castedExpression = (Expression<Func<User, bool>>)expression.CastParameter<User>(null);

            // Assert
            Assert.NotNull(castedExpression);
        }

        [Test]
        public void ReplacesMemberName()
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

        [Test]
        public void ReplacesParameter()
        {
            const string filter = "startswith(familyname, 'Tw')";
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

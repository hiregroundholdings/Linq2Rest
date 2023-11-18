using System.Net.Mail;

namespace LinqConvertTools.Tests.Fakes
{
    internal class EmailAddress
    {
        public EmailAddress(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public string? Name { get; set; }
    }

    internal class User : IQueryableUser
    {
        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        public ICollection<string> Roles { get; set; }

        public EmailAddress? EmailAddress { get; set; }

        string? IQueryableUser.EmailAddress => EmailAddress?.Value;

        string? IQueryableUser.FirstName => GivenName;
    }

    internal interface IQueryableUser
    {
        string? EmailAddress { get; }

        string? FirstName { get; }
    }
}

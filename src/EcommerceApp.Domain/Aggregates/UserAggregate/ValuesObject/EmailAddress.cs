using Domain.Aggregates.UserAggregate.Constants;
using Domain.Exceptions;
using Domain.SeedWorks.Events;
using System.Text.RegularExpressions;

namespace Domain.Aggregates.UserAggregate.ValuesObject
{
    public sealed class EmailAddress : ValueObject
    {
        public string Value { get; private set; } = string.Empty;

        public EmailAddress() { }

        public EmailAddress(string value)
        {
            this.Value = value.Trim();
        }

        public static EmailAddress Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException($"{UserMessages.EmailCannotEmpty}", nameof(value));

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase))
                throw new ValidationException($"{UserMessages.EmailInvalid}", nameof(value));

            return new EmailAddress(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

    }
}

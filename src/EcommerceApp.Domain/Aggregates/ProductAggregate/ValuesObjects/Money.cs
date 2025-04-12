using Domain.Aggregates.ProductAggregate.Constants;
using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Exceptions;
using Domain.SeedWorks.Events;

namespace Domain.Aggregates.ProductAggregate.ValuesObjects
{
    public sealed class Money : ValueObject, IEquatable<Money>
    {
        public decimal Amount { get; private set; }
        public CurrencyEnum Currency { get; private set; } = CurrencyEnum.VND;

        public Money() 
        {
            Amount = 0;
            Currency = CurrencyEnum.VND;
        }

        public Money(decimal amount, CurrencyEnum currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public Money(decimal amount)
        {
            Amount = amount;
            Currency = CurrencyEnum.VND;
        }

        public static Money Create(decimal amount)
        {
            if (amount < 0)
                throw new ValidationException($"{ProductMessages.AmountCannotNegative}", nameof(amount));

            return new Money(amount);
        }

        public static Money Create(decimal amount, CurrencyEnum currency)
        {
            if (amount < 0)
                throw new ValidationException($"{ProductMessages.AmountCannotNegative}", nameof(amount));

            return new Money(amount, currency);
        }

        public override bool Equals(object? obj)
            => obj is Money other && Amount == other.Amount && Currency == other.Currency;

        public bool Equals(Money? other)
            => other != null && Amount == other.Amount && Currency == other.Currency;

        public override int GetHashCode() => HashCode.Combine(Amount, Currency);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }

}

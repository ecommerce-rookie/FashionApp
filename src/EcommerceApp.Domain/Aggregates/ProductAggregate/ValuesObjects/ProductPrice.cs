using Domain.Aggregates.ProductAggregate.Constants;
using Domain.Aggregates.ProductAggregate.ValuesObjects;
using Domain.Exceptions;
using Domain.SeedWorks.Events;

namespace Domain.Aggregates.ProductAggregate.ValueObjects;

public sealed class ProductPrice : ValueObject
{
    public Money UnitPrice { get; private set; } = new Money();
    public Money PurchasePrice { get; private set; } = new Money();

    public ProductPrice() { }

    public ProductPrice(decimal unitPrice, decimal? purchasePrice)
    {
        UnitPrice = Money.Create(unitPrice);
        PurchasePrice = purchasePrice == null ? Money.Create(unitPrice) : Money.Create((decimal)purchasePrice);
    }

    public static ProductPrice Create(decimal unitPrice, decimal purchasePrice)
    {
        if (unitPrice < 0)
            throw new ValidationException($"{ProductMessages.UnitPriceCannotNegative}", nameof(UnitPrice));

        if (purchasePrice < 0)
            throw new ValidationException($"{ProductMessages.PurchasePriceCannotNegative}", nameof(PurchasePrice));

        if (purchasePrice > unitPrice)
            throw new ValidationException($"{ProductMessages.PurchasePriceCannotGreaterThanUnitPrice}", nameof(UnitPrice));

        return new ProductPrice(unitPrice, purchasePrice);
    }

    public static ProductPrice Create(decimal unitPrice)
    {
        if (unitPrice < 0)
            throw new ValidationException($"{ProductMessages.UnitPriceCannotNegative}", nameof(UnitPrice));

        return new ProductPrice(unitPrice, unitPrice);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return UnitPrice;
        yield return PurchasePrice;
    }
}
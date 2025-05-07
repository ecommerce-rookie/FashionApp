using Domain.Aggregates.OrderAggregate.Constants;
using Domain.Aggregates.ProductAggregate.ValuesObjects;
using Domain.Exceptions;
using Domain.SeedWorks.Events;

namespace Domain.Aggregates.OrderAggregate.ValuesObject
{
    public class Cart : ValueObject
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Money Price { get; set; } = new Money(0);

        public Cart() { }

        public Cart(Guid productId, int quantity, decimal price)
        {
            if(quantity <= 0)
            {
                throw new ValidationException($"{OrderMessage.QuantiyCannotBeNegative}", nameof(quantity));
            }

            ProductId = productId;
            Quantity = quantity;
            Price = new Money(price);
        }

        public static Cart Create(Guid productId, int quantity, decimal price)
        {
            return new Cart(productId, quantity, price);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ProductId;
            yield return Quantity;
            yield return Price;
        }
    }
}

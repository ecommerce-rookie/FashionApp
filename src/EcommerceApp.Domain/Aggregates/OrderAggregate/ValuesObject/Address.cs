using Domain.SeedWorks.Events;

namespace Domain.Aggregates.OrderAggregate.ValuesObject
{
    public class Address : ValueObject
    {
        public string? Street { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? Province { get; set; } = string.Empty;

        public Address() { }

        public Address(string? street, string? city, string? province)
        {
            Street = street;
            City = city;
            Province = province;
        }

        public static Address Create(string? street, string? city, string? province)
        {
            return new Address(street, city, province);
        }

        public override string ToString() => $"{Street}, {City}, {Province}";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ToString();
        }
    }
}

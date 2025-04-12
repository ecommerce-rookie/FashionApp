using Domain.Aggregates.ProductAggregate.Constants;
using Domain.Exceptions;
using Domain.SeedWorks.Events;

namespace Domain.Aggregates.ProductAggregate.ValuesObjects
{
    public class ImageUrl : ValueObject
    {
        public string Url { get; private set; } = string.Empty;

        public ImageUrl(string url)
        {
            Url = url;
        }

        public ImageUrl() { }

        public static ImageUrl Create(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ValidationException($"{ProductMessages.ImageUrlCannotEmpty}", nameof(url));
            
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new ValidationException($"{ProductMessages.ImageUrlInvalid}", nameof(url));
            
            return new ImageUrl(url);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Url;
        }

        public override string ToString()
        {
            return Url;
        }
    }
}

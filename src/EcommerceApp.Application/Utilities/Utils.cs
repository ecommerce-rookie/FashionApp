using Domain.Constants.Common;

namespace Application.Utilities
{
    public static class Utils
    {
        public static bool IsNewProduct(this DateTime createdAt)
        {
            // Get the date 30 days ago from now (New products)
            var date30DaysAgo = DateTime.UtcNow.AddDays(DefaultConstant.NewProductDays);

            // Check if the product was created within the last 30 days
            return createdAt > date30DaysAgo;
        }
    }
}

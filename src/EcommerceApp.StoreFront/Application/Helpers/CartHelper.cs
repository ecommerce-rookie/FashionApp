using StoreFront.Application.Extensions;

namespace StoreFront.Application.Helpers
{
    public static class CartHelper
    {
        public static int GetTotalItems(this HttpContext context)
        {
            var totalItems = 0;
            // if authen get from session
            if (context.Session.IsAuthenticated())
            {
                totalItems = context.Session.GetNumberOfProduct();
            } else
            {
                // if not authen get from cookie
                var productIds = context.Request.GetCookie<IDictionary<Guid, int>>("cart");
                if (productIds != null && productIds.Any())
                {
                    totalItems += productIds.Sum(x => x.Value);
                }
            }

            return totalItems;
        }
    }
}

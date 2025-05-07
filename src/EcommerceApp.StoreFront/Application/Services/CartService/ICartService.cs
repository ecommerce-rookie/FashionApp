namespace StoreFront.Application.Services.CartService
{
    public interface ICartService
    {
        Task AddItemToCart(string userId, string productId, int quantity);
        Task<bool> RemoveItemFromCart(string userId, string productId, int quantity, bool? isDeleteAll);
        Task AddItemsToCart(string userId, IDictionary<Guid, int> productIds);
        Task<IDictionary<Guid, int>> GetCarts(string userId);
        Task<int> CountProduct(string userId);
        Task<bool> ClearCart(string userId);
    }
}

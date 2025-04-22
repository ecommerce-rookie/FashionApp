using StackExchange.Redis;

namespace StoreFront.Application.Services.CartService
{
    public class CartService : ICartService
    {
        protected readonly IDatabaseAsync _database;
        private readonly string _keyPrefix = "carts";

        public CartService(IConnectionMultiplexer connectionMultiplexer)
        {
            _database = connectionMultiplexer.GetDatabase();
        }

        protected string GetKey(string id) => $"{_keyPrefix}:{id}";

        public async Task AddItemsToCart(string userId, IDictionary<Guid, int> productIds)
        {
            var key = GetKey(userId);
            var entries = productIds
                .Select(x => new HashEntry(x.Key.ToString(), x.Value))
                .ToArray();

            await _database.HashSetAsync(key, entries); 
        }

        public async Task AddItemToCart(string userId, string productId, int quantity)
        {
            var key = GetKey(userId);
           
            await _database.HashIncrementAsync(key, productId, quantity);
        }

        public async Task<bool> RemoveItemFromCart(string userId, string productId, int quantity, bool? isDeleteAll)
        {
            var key = GetKey(userId.ToString());

            if (isDeleteAll.HasValue && isDeleteAll.Value)
            {
                // Remove the item from the cart
                return await _database.HashDeleteAsync(key, productId);
            } else
            {
                // Decrease the quantity of the item in the cart
                var newQty = await _database.HashDecrementAsync(key, productId, quantity);
                if(newQty <= 0)
                {
                    // If the quantity is less than or equal to 0, remove the item from the cart
                    return await _database.HashDeleteAsync(key, productId);
                }

                return true;
            }
        }

        public async Task<int> CountProduct(string userId)
        {
            var key = GetKey(userId);

            var entries = await _database.HashGetAllAsync(key);

            var count = entries
                .Where(e => e.Value.HasValue)
                .Sum(e => (int)e.Value);

            return count;
        }

        public async Task<IDictionary<Guid, int>> GetCarts(string userId)
        {
            var key = GetKey(userId);
            var entries = await _database.HashGetAllAsync(key);

            var cart = entries
                .Where(e => e.Value.HasValue)
                .ToDictionary(
                    e => Guid.Parse(e.Name.ToString()),
                    e => (int)e.Value
                );

            return cart;
        }
    }
}

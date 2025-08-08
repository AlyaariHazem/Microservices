

namespace Basket.API.Data
{
    
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _jsonOptions =
            new(JsonSerializerDefaults.Web);

        public BasketRepository(IDistributedCache cache) => _cache = cache;

        /// <inheritdoc/>
        public async Task<ShoppingCart?> GetBasket(
            string userName,
            CancellationToken cancellationToken)
        {
            var cached = await _cache.GetStringAsync(userName, cancellationToken);

            return string.IsNullOrWhiteSpace(cached)
                ? null
                : JsonSerializer.Deserialize<ShoppingCart>(cached, _jsonOptions);
        }

        /// <inheritdoc/>
        public async Task<ShoppingCart> StoreBasket(
            ShoppingCart basket,
            CancellationToken cancellationToken)
        {
            var json = JsonSerializer.Serialize(basket, _jsonOptions);

            await _cache.SetStringAsync(
                basket.UserName,
                json,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                },
                cancellationToken);

            return await GetBasket(basket.UserName, cancellationToken)
                   ?? basket;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteBasket(
            string userName,
            CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync(userName, cancellationToken);
            return true;
        }
    }
}
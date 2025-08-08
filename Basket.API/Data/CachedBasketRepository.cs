
namespace Basket.API.Data
{
    public class CachedBasketRepository(IBasketRepository repository) : IBasketRepository
    {
        public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
        {
            return await repository.DeleteBasket(userName, cancellationToken);
        }

        public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellationToken)
        {
            return await repository.GetBasket(userName, cancellationToken);
        }

        public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken)
        {
            return await repository.StoreBasket(basket, cancellationToken);
        }
    }
}

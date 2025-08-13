using Catalog.API;

namespace Basket.API.Basket.GetBasket
{
    public record GetBasketByIdQuery(string UserName)
        : IQuery<GetBasketByIdResult>;

    public record GetBasketByIdResult(ShoppingCart Basket);

    public class GetBasketHandler
        : IQueryHandler<GetBasketByIdQuery, GetBasketByIdResult>
    {
        private readonly IBasketRepository _repository;

        public GetBasketHandler(IBasketRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetBasketByIdResult> Handle(
            GetBasketByIdQuery query,
            CancellationToken cancellationToken)
        {
            var basket = await _repository.GetBasket(query.UserName, cancellationToken);

            if (basket is null)
            {
                // Handle not found — either throw, return null, or let pipeline handle
                throw new NotFoundException($"Basket for user '{query.UserName}' not found.");
            }

            return new GetBasketByIdResult(basket);
        }
    }
}

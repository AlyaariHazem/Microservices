
namespace Basket.API.Basket.GetBasket
{
    public record GetBasketByIdQuery(Guid Id)
       : IQuery<GetBasketByIdResult>;

    public record GetBasketByIdResult(ShoppingCart Basket);
    public class GetBasketHandler
    {
    }
}

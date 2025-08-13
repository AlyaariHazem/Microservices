
namespace Basket.API.Basket.GetBasket
{
    public record GetBasketByIdResponse(ShoppingCart Basket);
    public class GetBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/baskets/{userName}", async (string userName, ISender sender) =>
            {
                var result = await sender.Send(new GetBasketByIdQuery(userName));
                var response = new GetBasketByIdResponse(result.Basket);
                return Results.Ok(response);
            })
            .WithName("GetBasketByUserName")
            .Produces<GetBasketByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Basket by UserName")
            .WithDescription("Returns the basket for the specified user.");
        }
    }
}

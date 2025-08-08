
namespace Basket.API.Basket.GetBasket
{
    public record GetBasketByIdResponse(ShoppingCart Basket);
    public class GetBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/Baskets/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetBasketByIdQuery(id));
                var response = new GetBasketByIdResponse(result.Basket);

                return Results.Ok(response);
            })
            .WithName("GetBasketById")
            .Produces<GetBasketByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Basket by Id")
            .WithDescription("Returns the Basket that matches the supplied Guid.");
        }
    }
}

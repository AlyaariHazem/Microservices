namespace Catalog.API.Products.GetProductById
{
    public record GetProductByIdResponse(Product Product);

    public sealed class GetProductByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetProductByIdQuery(id));
                var response = new GetProductByIdResponse(result.Product);

                return Results.Ok(response);
            })
            .WithName("GetProductById")
            .Produces<GetProductByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get product by Id")
            .WithDescription("Returns the product that matches the supplied Guid.");
        }
    }
}

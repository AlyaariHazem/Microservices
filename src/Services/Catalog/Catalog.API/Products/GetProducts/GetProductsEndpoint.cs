
namespace Catalog.API.Products.GetProducts
{
    public record GetProductsRequest(int? pageNumber = 1, int? pageSize = 10);
    public record GetProductsResponse(IEnumerable<Product> products);
    public class GetProductsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async ([AsParameters] GetProductsRequest request, ISender sender) =>
            {
                var query = new GetProductsQuery(request.pageNumber, request.pageSize);
                var result = await sender.Send(query);
                var response = new GetProductsResponse(result.products);
                return Results.Ok(response);
            })
                .WithName("GetProducts")
                .Produces<GetProductsResponse>(StatusCodes.Status200OK)
                .WithSummary("Get Products")
                .WithDescription("Get Products");
        }
    }
}

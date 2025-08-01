
namespace Catalog.API.Products.UpdateProducts
{
    public record UpdateProductRequest(Guid Id, string Name, List<string> Category, string Description, string ImageFile, decimal Price);
    public record UpdateProductResponse(bool IsSuccess);
    public class UpaateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/products", async (UpdateProductRequest request, ISender sender) =>
            {
                var command = request.Adapt<UpdateProductCommand>();
                var result = await sender.Send(command);
                var response = new UpdateProductResponse(result.IsSuccess);
                return Results.Ok(response);
            })
                .WithName("UpdateProduct")
                .Produces<UpdateProductResponse>()
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithSummary("Update an existing product")
                .WithDescription("Updates the details of an existing product in the catalog.");
        }
    }
}
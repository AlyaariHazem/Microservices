
namespace Catalog.API.Products.UpdateProducts
{
    public record UpdateProductRequest(Guid Id, string Name, List<string> Category, string Description, string ImageFile, decimal Price);
    public record UpdateProductResponse(bool IsSuccess);
    public class UpaateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/products/{id:guid}", async (
                   Guid id,
                   UpdateProductRequest body,
                   ISender sender) =>
            {
                var command = new UpdateProductCommand(
                    id, body.Name, body.Category,
                    body.Description, body.ImageFile, body.Price);

                var result = await sender.Send(command);
                return Results.Ok(new UpdateProductResponse(result.IsSuccess));
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
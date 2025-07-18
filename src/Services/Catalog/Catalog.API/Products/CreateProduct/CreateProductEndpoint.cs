public record CreateProductRequest(string Name, List<string> Category, string Description, string ImageFile, decimal Price);
public record CreateProductResponse(Guid Id);

namespace Catalog.API.Products.CreateProduct
{
    public class CreateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/products", async (CreateProductRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateProductCommand>();
                var result = await sender.Send(command);

                var response = new CreateProductResponse(result.Id);

                return Results.Created($"/products/{result.Id}", response);
            })
            .WithName("CreateProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a new product")
            .WithDescription("Create Product.");
        }
    }
}

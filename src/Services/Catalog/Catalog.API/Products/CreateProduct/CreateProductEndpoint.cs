
public record CreateProductRequest(string Name, List<string> Catelog, string Description,string ImageFile, decimal Price);
public record CreateProductResponse(Guid id);

namespace Catalog.API.Products.CreateProduct
{
    public class CreateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/products",async (CreateProductRequest request,ISender sender) =>
            {
                var command = request.Adapt<CreateProductCommand>();

                var result = await sender.Send(command);
                var response = request.Adapt<CreateProductResponse>();

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

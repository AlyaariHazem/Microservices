
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Products.GetProductByCategory
{
    public record GetProductByCategoryRequest(int? pageNumber = 1, int? pageSize = 10);
    public record GetProductByCategoryResponse(IEnumerable<Product> Products);
    
    public class GetProductByCategoryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/category/{category}", async (
                string category,
                [AsParameters] GetProductByCategoryRequest request,
                ISender sender) =>
            {
                var query = new GetProductByCategoryQuery(category, request.pageNumber, request.pageSize);
                var result = await sender.Send(query);
                var response = new GetProductByCategoryResponse(result.Products);
                return Results.Ok(response);
            })
            .WithName("GetProductByCategory")
            .Produces<GetProductByCategoryResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Products by Category")
            .WithDescription("Retrieve a paginated list of products filtered by category.");
        }
    }
}

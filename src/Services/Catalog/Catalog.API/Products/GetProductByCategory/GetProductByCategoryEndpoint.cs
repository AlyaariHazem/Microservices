using BuildingBlocks.Querying.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Products.GetProductByCategory
{
    public record GetProductByCategoryResponse(PagedResult<Product> Products);
    
    public class GetProductByCategoryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/category/{category}", async (
                string category,
                [FromQuery] string? search,
                [FromQuery] string? sortBy,
                [FromQuery] bool? sortDesc,
                [FromQuery] int? page,
                [FromQuery] int? pageSize,
                HttpContext httpContext,
                ISender sender) =>
            {
                // Extract filters from query string
                // Supports patterns like: ?filterName=value or ?filter[name]=value
                var filters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var standardParams = new HashSet<string>(StringComparer.OrdinalIgnoreCase) 
                { 
                    "search", "sortBy", "sortDesc", "page", "pageSize" 
                };
                
                foreach (var queryParam in httpContext.Request.Query)
                {
                    var key = queryParam.Key;
                    // Skip standard parameters
                    if (standardParams.Contains(key))
                        continue;
                    
                    // Handle filter[name]=value pattern
                    if (key.StartsWith("filter[", StringComparison.OrdinalIgnoreCase) && 
                        key.EndsWith("]"))
                    {
                        var filterKey = key.Substring(7, key.Length - 8);
                        if (!string.IsNullOrWhiteSpace(filterKey) && queryParam.Value.Count > 0)
                        {
                            filters[filterKey] = queryParam.Value[0]!;
                        }
                    }
                    // Handle filterName=value pattern
                    else if (key.StartsWith("filter", StringComparison.OrdinalIgnoreCase) && 
                             key.Length > 6 && char.IsUpper(key[6]))
                    {
                        var filterKey = key.Substring(6);
                        if (!string.IsNullOrWhiteSpace(filterKey) && queryParam.Value.Count > 0)
                        {
                            filters[filterKey] = queryParam.Value[0]!;
                        }
                    }
                }
                
                // Add category to filters
                filters["category"] = category;
                
                var queryParams = new QueryParameters
                {
                    Search = search,
                    SortBy = sortBy,
                    SortDesc = sortDesc ?? false,
                    Page = page ?? 1,
                    PageSize = pageSize ?? 20,
                    Filters = filters
                };
                
                var query = new GetProductByCategoryQuery(queryParams);
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

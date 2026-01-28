using BuildingBlocks.Querying.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Products.GetProducts
{
    public record GetProductsResponse(PagedResult<Product> Products);
    
    public class GetProductsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async (
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
                        var filterKey = key.Substring(7, key.Length - 8); // Extract name from filter[name]
                        if (!string.IsNullOrWhiteSpace(filterKey) && queryParam.Value.Count > 0)
                        {
                            filters[filterKey] = queryParam.Value[0]!;
                        }
                    }
                    // Handle filterName=value pattern (for backward compatibility)
                    else if (key.StartsWith("filter", StringComparison.OrdinalIgnoreCase) && 
                             key.Length > 6 && char.IsUpper(key[6]))
                    {
                        var filterKey = key.Substring(6); // Remove "filter" prefix
                        if (!string.IsNullOrWhiteSpace(filterKey) && queryParam.Value.Count > 0)
                        {
                            filters[filterKey] = queryParam.Value[0]!;
                        }
                    }
                }
                
                var parameters = new QueryParameters
                {
                    Search = search,
                    SortBy = sortBy,
                    SortDesc = sortDesc ?? false,
                    Page = page ?? 1,
                    PageSize = pageSize ?? 20,
                    Filters = filters
                };
                
                var query = new GetProductsQuery(parameters);
                var result = await sender.Send(query);
                var response = new GetProductsResponse(result.Products);
                return Results.Ok(response);
            })
                .WithName("GetProducts")
                .Produces<GetProductsResponse>(StatusCodes.Status200OK)
                .WithSummary("Get Products")
                .WithDescription("Get paginated, filterable, and sortable list of products. Use query parameters: ?search=term&sortBy=name&sortDesc=true&page=1&pageSize=20&filterName=value&filterPrice=100");
        }
    }
}

using BuildingBlocks.Querying.Extensions;
using BuildingBlocks.Querying.Models;
using BuildingBlocks.Querying.Security;

namespace Catalog.API.Products.GetProductByCategory
{
    public record GetProductByCategoryQuery(QueryParameters Parameters) 
        : IQuery<GetProductByCategoryResult>;
    
    public record GetProductByCategoryResult(PagedResult<Product> Products);
    
    internal class GetProductByCategoryQueryHandler(IDocumentSession session)
        : IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
    {
        private static readonly ColumnMap<Product> ProductColumns = new ColumnMap<Product>()
            .Add("name", p => p.Name)
            .Add("description", p => p.Description)
            .Add("price", p => p.Price)
            .Add("category", p => string.Join(",", p.Categories));

        public async Task<GetProductByCategoryResult> Handle(
            GetProductByCategoryQuery query, 
            CancellationToken cancellationToken)
        {
            var productsQuery = session.Query<Product>();
            
            // Apply category filter if present (handle manually since Categories is a List<string>)
            if (query.Parameters.Filters.TryGetValue("category", out var category))
            {
                productsQuery = (Marten.Linq.IMartenQueryable<Product>)productsQuery.Where(p => p.Categories.Contains(category));
            }
            
            // Remove category from filters to avoid double application
            var filtersWithoutCategory = new Dictionary<string, string>(
                query.Parameters.Filters.Where(f => !f.Key.Equals("category", StringComparison.OrdinalIgnoreCase)),
                StringComparer.OrdinalIgnoreCase);
            
            var queryParamsWithoutCategory = query.Parameters with { Filters = filtersWithoutCategory };
            
            // Apply other query parameters and paginate using the extension method
            var pagedList = await productsQuery.ToPagedListAsync(
                queryParamsWithoutCategory,
                ProductColumns,
                cancellationToken);
            
            var pagedResult = pagedList.ToPagedResult();
            
            return new GetProductByCategoryResult(pagedResult);
        }
    }
}

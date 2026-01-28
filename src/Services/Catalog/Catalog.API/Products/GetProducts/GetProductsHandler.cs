using BuildingBlocks.Querying.Extensions;
using BuildingBlocks.Querying.Models;
using BuildingBlocks.Querying.Security;

namespace Catalog.API.Products.GetProducts
{
    public record GetProductsQuery(QueryParameters Parameters) : IQuery<GetProductsResult>;
    
    public record GetProductsResult(PagedResult<Product> Products);
    
    internal class GetProductsQueryHandler(IDocumentSession session)
        : IQueryHandler<GetProductsQuery, GetProductsResult>
    {
        private static readonly ColumnMap<Product> ProductColumns = new ColumnMap<Product>()
            .Add("name", p => p.Name)
            .Add("description", p => p.Description)
            .Add("price", p => p.Price)
            .Add("category", p => string.Join(",", p.Categories));

        public async Task<GetProductsResult> Handle(
            GetProductsQuery query,
            CancellationToken cancellationToken)
        {
            var productsQuery = session.Query<Product>();
            
            var pagedList = await productsQuery.ToPagedListAsync(
                query.Parameters,
                ProductColumns,
                cancellationToken);
            
            var pagedResult = pagedList.ToPagedResult();
            
            return new GetProductsResult(pagedResult);
        }
    }
}

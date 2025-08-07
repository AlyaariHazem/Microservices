using Catalog.API.Exception;
using Marten.Pagination;

namespace Catalog.API.Products.GetProducts
{
    public record GetProductsQuery(int? pageNumber = 1, int? pageSize = 10) : IQuery<GetProductsResult>;
    public record GetProductsResult(IEnumerable<Product> products);
    internal class GetProductsQueryHandler
        (IDocumentSession session)
        :IQueryHandler<GetProductsQuery,GetProductsResult>
    {
        public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            
            var products = await session.Query<Product>().ToPagedListAsync(query.pageNumber ?? 1,query.pageSize ?? 10,cancellationToken);
            
            return new GetProductsResult(products);
        }
    }
}

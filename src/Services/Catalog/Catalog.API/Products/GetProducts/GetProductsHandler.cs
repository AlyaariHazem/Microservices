namespace Catalog.API.Products.GetProducts
{
    public record GetProductsQuery():IQuery<GetProductsResult>;
    public record GetProductsResult(IEnumerable<Product> products);
    internal class GetProductsQueryHandler
    {
    }
}

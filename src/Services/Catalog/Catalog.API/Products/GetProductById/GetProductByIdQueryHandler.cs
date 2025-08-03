using Catalog.API.Exception;
using CatalogAPI.Products.DeleteProduct;

namespace Catalog.API.Products.GetProductById
{
    // ───────── Query & Result ──────────────────────────────────────────────
    public record GetProductByIdQuery(Guid Id)
        : IQuery<GetProductByIdResult>;

    public record GetProductByIdResult(Product Product);

    public class GetByIdProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public GetByIdProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id Is Required");
        }
    }
    // ───────── Handler ─────────────────────────────────────────────────────
    internal class GetProductByIdQueryHandler(
        IDocumentSession session,
        ILogger<GetProductByIdQueryHandler> logger)
        : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
    {
        public async Task<GetProductByIdResult> Handle(
            GetProductByIdQuery query,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Get product by Id: {Id}", query.Id);

            var product = await session.LoadAsync<Product>(query.Id, cancellationToken);

            if (product is null)
                throw new ProductNotFoundException(query.Id);
            

            return new GetProductByIdResult(product);
        }
    }
}

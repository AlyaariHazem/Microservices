

namespace Catalog.API.Products.UpdateProducts
{
    public record UpdateProductCommand(Guid Id, string Name, List<string> Category, string Description,string ImageFile, decimal Price)
        :ICommand<UpdateProductResult>;
    public record UpdateProductResult(bool IsSuccess);
    public class UpdateProductHandler 
        (IDocumentSession session, ILogger<UpdateProductHandler> logger)
        : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {
        public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("UpdateProductHandler called with command: {@Command}", command);
            var product = await session.LoadAsync<Product>(command.Id, cancellationToken);
            if (product is not null)
            {
                logger.LogWarning("Product with ID {Id} not found", command.Id);
                return new UpdateProductResult(false);
            }

            product.Name = command.Name;
            product.Categories = command.Category;
            product.Description = command.Description;
            product.ImageFile = command.ImageFile;
            product.Price = command.Price;
            session.Update(product);
            await session.SaveChangesAsync(cancellationToken);
            return new UpdateProductResult(true);

        }
    }
}


namespace Catalog.API.Products.CreateProduct
{
    // Command definition
    public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price)
        : ICommand<CreateProductResult>;

    // Result definition
    public record CreateProductResult(Guid Id);

    // Command handler implementation
    internal class CreateProductCommandHandler(IDocumentSession sesstion)
        : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        // This method will handle the command
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {

            // For the sake of this example, assume the product is created and we return the new product Id
            // In real scenarios, you would save the product in a database

            var product = new Product
            {
                Name = command.Name,
                Categories = command.Category,
                Description = command.Description,
                ImageFile = command.ImageFile,
                Price = command.Price,

            };
            //save to database
            sesstion.Store(product);
            await sesstion.SaveChangesAsync(cancellationToken);

            return new CreateProductResult(product.Id);
        }
    }
}

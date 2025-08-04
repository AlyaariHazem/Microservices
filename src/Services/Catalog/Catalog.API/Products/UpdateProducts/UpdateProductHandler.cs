

namespace Catalog.API.Products.UpdateProducts
{
    public record UpdateProductCommand(Guid Id, string Name, List<string> Category, string Description,string ImageFile, decimal Price)
        :ICommand<UpdateProductResult>;
    public record UpdateProductResult(bool IsSuccess);
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(command => command.Id).NotEmpty().WithMessage("Product Id is Required");
            RuleFor(command => command.Name).NotEmpty().WithMessage("Product name is required.");
            RuleFor(command => command.Category).NotEmpty().WithMessage("At least one category is required.");
            RuleFor(command => command.Description).NotEmpty().WithMessage("Product description is required.");
            RuleFor(command => command.ImageFile).NotEmpty().WithMessage("Image file is required.");
            RuleFor(command => command.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
        }
    }
    public class UpdateProductHandler 
        (IDocumentSession session)
        : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {
        public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var product = await session.LoadAsync<Product>(command.Id, cancellationToken);
            if (product is null)
            {
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

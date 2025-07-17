using BuildingBlocks.CQRS;
using Catalog.API.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog.API.Products.CreateProduct
{
    // Command definition
    public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price)
        : ICommand<CreateProductResult>;

    // Result definition
    public record CreateProductResult(Guid Id);

    // Command handler implementation
    public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        // This method will handle the command
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            // Logic to create the product
            var newProductId = Guid.NewGuid();

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

            return new CreateProductResult(Guid.NewGuid());
        }
    }
}

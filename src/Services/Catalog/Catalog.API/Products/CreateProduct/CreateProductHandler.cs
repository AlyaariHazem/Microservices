using BuildingBlocks.CQRS;
using Catalog.API.Models;
using MediatR;
using Marten;
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
    internal class CreateProductCommandHandler(IDocumentSession session)
        : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = command.Name,
                Categories = command.Category,
                Description = command.Description,
                ImageFile = command.ImageFile,
                Price = command.Price
            };

            session.Store(product);
            await session.SaveChangesAsync(cancellationToken);

            return new CreateProductResult(product.Id);
        }
    }
}

namespace Catalog.API.Data
{
    public class CatalogIntialData : IInitialData
    {
        public async Task Populate(IDocumentStore store, CancellationToken cancellation)
        {
            using var session = store.LightweightSession();
            if(await session.Query<Product>().AnyAsync(cancellation))
                return;
            session.Store<Product>(GetProductFiguredProducts());
            await session.SaveChangesAsync(cancellation);
        }
        private static IEnumerable<Product> GetProductFiguredProducts() => new List<Product>
        {
             new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Figured Products",
                Description = "A collection of figured products.",
                Price = 19.99m,
                ImageFile = "figured-products.jpg",
                Categories = new List<string> { "Figured", "Products" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Figured Product 1",
                Description = "Description for figured product 1.",
                Price = 29.99m,
                ImageFile = "figured-product-1.jpg",
                Categories = new List<string> { "Figured", "Product" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Figured Product 2",
                Description = "Description for figured product 2.",
                Price = 39.99m,
                ImageFile = "figured-product-2.jpg",
                Categories = new List<string> { "Figured", "Product" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Figured Product 3",
                Description = "Description for figured product 3.",
                Price = 49.99m,
                ImageFile = "figured-product-3.jpg",
                Categories = new List<string> { "Figured", "Product" }
            }
        };
        
    }
}

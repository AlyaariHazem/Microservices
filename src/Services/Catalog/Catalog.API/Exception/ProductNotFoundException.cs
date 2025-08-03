namespace Catalog.API.Exception
{
    public class ProductNotFoundException : NotFoundException
    {
        public ProductNotFoundException(Guid Id) : base("Product", Id)
        {

        }
        
    }
}

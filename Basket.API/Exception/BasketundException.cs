namespace Catalog.API.Exception
{
    public class BasketundException : NotFoundException
    {
        public BasketundException(Guid Id) : base("Product", Id)
        {

        }
        
    }
}

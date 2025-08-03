namespace Catalog.API
{
    public class NotFoundException: Exception
    {
        public NotFoundException(string message)
            : base(message)
        {
        }
        public NotFoundException(string resourceName, object key)
            : base($"Entity '{resourceName}' with key '{key}' was not found.")
        {
        }
    }
}

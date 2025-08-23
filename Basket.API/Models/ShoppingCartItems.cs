namespace Basket.API.Models
{
    public class ShoppingCartItems
    {
        public Guid ProductId { get; set; }

        /* A copy of the product’s name for quick rendering */
        public string ProductName { get; set; } = string.Empty;
        
        /* Thumbnail or small image to show in the basket view */
        public string ImageFile { get; set; } = string.Empty;

        /* Unit price when the item was added (to keep history stable) */
        public decimal UnitPrice { get; set; }

        /* How many units the user put in the basket */
        public int Quantity { get; set; }

        /* Calculated helper for convenience */
        public decimal Price { get; set; } = default;
    }
}

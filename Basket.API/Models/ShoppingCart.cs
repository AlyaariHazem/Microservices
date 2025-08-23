namespace Basket.API.Models
{
    public class ShoppingCart
    {
        public string UserName { get; set; } = string.Empty;

        /* All line items */
        public List<ShoppingCartItems> Items { get; set; } = new();

        /* Optional promo / coupon code that the user applied */
        public string? CouponCode { get; set; }

        /* Convenience aggregate – re-calculated on every get */
        public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);

        public ShoppingCart() { }

        public ShoppingCart(string userName)
        {
            UserName = userName;
        }
    }
}

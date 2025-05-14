namespace Madhu.Models
{
    public class Product_Item
    {
        public Product Product { get; set; } // Reference to a Product object
        public string ImageUrl { get; set; }
        public int Quantity { get; set; } // Quantity of the product added to cart

        private decimal _SubTotal;
        public decimal SubTotal
        {
            get { return _SubTotal; }
            set { _SubTotal = Product.Price * Quantity; } // Calculate subtotal dynamically
        }
    }
}

namespace Madhu.Models
{
    public class CartItem
    {

        public int Id { get; set; }
        public string Username { get; set; } // Links the cart to a user
        public string ProductSku { get; set; } // Matches [ProductSku] in DB
        public int Quantity { get; set; } // Matches [Quantity] in DB
        public int Amount { get; set; } // Matches [Amount] in DB
        public string Status { get; set; } = "Added To Cart";

    }
}

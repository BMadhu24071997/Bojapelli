using System.ComponentModel.DataAnnotations;

namespace Madhu.Models
{
    public class ShippingDetails
    {

        public string Username { get; set; }
        public string Address { get; set; }
        public long Mobile { get; set; }

        [Key]
        public string Email { get; set; }


    }
}

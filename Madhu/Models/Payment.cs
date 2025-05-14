using System.ComponentModel.DataAnnotations;

namespace Madhu.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public string Username { get; set; } // Assuming payments are linked to users
        public string OrderStatus { get; set; } = "";


        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public int Amount { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime PaymentDate { get; set; }
    }
}

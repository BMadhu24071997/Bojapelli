using System.ComponentModel.DataAnnotations;

namespace Madhu.Models
{
    public class TrackAllOrders
    {
        [Key]
        public int PaymentId { get; set; }
        public int Amount { get; set; }
        public string OrderStatus { get; set; } = "";
        public DateTime StatusUpdated { get; set; } = DateTime.Now;


    }
}

using System.ComponentModel.DataAnnotations;

namespace Madhu.Models
{
    public class MyGiftVoucher
    {
        [Key]
        public string VoucherId { get; set; }
        public string Status { get; set; }
        public string UsedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime UsedDatetime { get; set; }
        public int Amount { get; set; }

    }
}

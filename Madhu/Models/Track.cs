using System.ComponentModel.DataAnnotations;

namespace Madhu.Models
{
    public class Track
    {
        [Key]
        public int OrderId { get; set; }
        public string Status { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }


    }
}

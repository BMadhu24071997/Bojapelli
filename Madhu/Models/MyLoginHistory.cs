using System.ComponentModel.DataAnnotations;

namespace Madhu.Models
{
    public class MyLoginHistory
    {
        [Key]
        public string UserName { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;

        public string IpAddress { get; set; }

        public string UserAction { get; set; }
    }
}
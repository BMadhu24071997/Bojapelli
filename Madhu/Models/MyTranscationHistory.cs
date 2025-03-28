using System.ComponentModel.DataAnnotations;

namespace Madhu.Models
{
    public class MyTranscationHistory
    {

        [Key]

        public int TranscationId { get; set; }

        public string FromUsername { get; set; }
        public string ToUsername { get; set; }

        public int Amount { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;

        public string IpAddress { get; set; }

    }
}

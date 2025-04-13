using System.ComponentModel.DataAnnotations;

namespace Madhu.Models
{
    public class EmailVerification
    {
        [Key]
        public string UserName { get; set; }
        public string Email { get; set; }
        public int OTP { get; set; }
        public DateTime OTPGeneratedDateTime { get; set; } = DateTime.Now;


    }
}

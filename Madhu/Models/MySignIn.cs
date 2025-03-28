using System.ComponentModel.DataAnnotations;

namespace Madhu.Models
{
    public class MySignIn
    {
        [Key]
        public string UserName { get; set; }
        public string Password { get; set; }

    }
}

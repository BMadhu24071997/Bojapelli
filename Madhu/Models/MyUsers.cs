using Humanizer;
using System.ComponentModel.DataAnnotations;

namespace Madhu.Models
{
    public class MyUsers
    {
        [Key]
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateofBirth { get; set; }
        public bool Admin { get; set; } = false;
        public bool ContentEditor { get; set; } = false;
        public DateTime CreatedDatetime { get; set; } = DateTime.Now;
        public string Location { get; set; }
        public DateTime Lastupdatedtime { get; set; } = DateTime.Now;
        public bool Nationality { get; set; }
        public string Gender { get; set; }
        public int AccountBalance { get; set; } = 0;
        public long Mobile { get; set; }

        public string UserStatus { get; set; } = "New";
        public string EmailStatus { get; set; } 
    }
}

using System.ComponentModel.DataAnnotations;

namespace Madhu.Models
{
    public class MyLoanRequest
    {

        [Key]
        public int LoanId { get; set; }
        public string WhomRequested { get; set; }
        public int LoanAmount { get; set; }
        public string UserComment { get; set; }
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;
  
        public string LoanRequestStatus { get; set; } = "Pending";
    }
}

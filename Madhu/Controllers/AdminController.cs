using Madhu.Models;
using Microsoft.AspNetCore.Mvc;

namespace Madhu.Controllers
{
    public class AdminController : Controller
    {


        private readonly ApplicationDbContext _db;
        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AllLoanRequests()
        {
            IEnumerable<MyLoanRequest> myloanrequest = _db.LoanRequest;
            return View(myloanrequest);

        }

        // Approving loan for user based on Loan ID
        public IActionResult LoanApprove(int? LoanID)
        {
            // finding Loan details using Loan ID
            var _LoanDetails = _db.LoanRequest.Find(LoanID);

            // finding user
            var _user = _db.Users.Find(_LoanDetails.WhomRequested);

            _user.AccountBalance = _user.AccountBalance + _LoanDetails.LoanAmount;
            _db.Update(_user);
            _db.SaveChanges();

            // changing Loan status to Approved
            _LoanDetails.LoanRequestStatus = "Approved";
            _db.Update(_LoanDetails);
            _db.SaveChanges();

            return RedirectToAction("AllLoanRequests", "Admin");

        }
        public IActionResult LoanReject(int? LoanID)
        {
            var _LoanDetails = _db.LoanRequest.Find(LoanID);

            _LoanDetails.LoanRequestStatus = "Reject";

            _db.Update(_LoanDetails);

            _db.SaveChanges();


            return RedirectToAction("AllLoanRequests", "Admin");


        }
        public IActionResult GenerateGiftVoucher()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GenerateGiftVoucher(MyGiftVoucher isVoucherValid)
        {

            try
            {


                var rand = new Random();
                isVoucherValid.VoucherId = rand.Next(10000) + "-" + isVoucherValid.Amount;

                isVoucherValid.Status = "Not Used";
                isVoucherValid.UsedBy = "NA";
                isVoucherValid.CreatedDateTime = DateTime.Now;
                isVoucherValid.UsedDatetime = DateTime.Now;
                _db.GiftVoucher.Add(isVoucherValid);
                _db.SaveChanges();
                return RedirectToAction("Index", "Dashboard");


            }
            catch
            {
                return View();
            }

        }
        public IActionResult SeeAllGeneratedVouchers()
        {
            IEnumerable<MyGiftVoucher> mygiftvoucher = _db.GiftVoucher.OrderByDescending(u => u.CreatedDateTime);
            return View(mygiftvoucher);

        }

        public IActionResult UserApprove(string? Username)
        {
            var _user = _db.Users.Find(Username);
            _user.UserStatus = "Active";
            _db.Users.Update(_user);
            _db.SaveChanges();
            return RedirectToAction("Admin", "Dashboard");

        }
        public IActionResult UserSuspend(string? Username)
        {
            var _user = _db.Users.Find(Username);
            _user.UserStatus = "Suspended";
            _db.Users.Update(_user);
            _db.SaveChanges();
            return RedirectToAction("Admin", "Dashboard");

        }
        public IActionResult UserUnSuspend(string? Username)
        {
            var _user = _db.Users.Find(Username);
            _user.UserStatus = "Active";
            _db.Users.Update(_user);
            _db.SaveChanges();
            return RedirectToAction("Admin", "Dashboard");

        }

    }
}





using Madhu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Net;

namespace Madhu.Controllers
{
    public class DashboardController : Controller
    {

        private readonly ApplicationDbContext _db;
        public DashboardController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            try
            {
                var isUserExists = _db.Users.Find(HttpContext.Session.GetString("UserName")); // getting user details using session
                return View(isUserExists);


            }
            catch
            {
                return View();
            }


        }
        public IActionResult Edit()
        {
            var user = _db.Users.Find(HttpContext.Session.GetString("UserName"));
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(MyUsers obj)
        {
            try
            {
                _db.Users.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index", "Dashboard");
            }
            catch
            {
                return View("Oops", "Home");
            }

        }
        public IActionResult Delete()                               //delete the user from Database
        {
            try
            {
                var deleteData = _db.Users.Find(HttpContext.Session.GetString("UserName"));
                _db.Users.Remove(deleteData);
                _db.SaveChanges();


                HttpContext.Session.SetString("SignIn", "False");
                HttpContext.Session.SetString("UserName", "");
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View("Oops", "Home");
            }
        }

        //Transfer Money Page
        public IActionResult MoneyTransfer()
        {
            MyMoneyTransfer obj = new MyMoneyTransfer();
            obj.FromUser = HttpContext.Session.GetString("UserName");
            return View(obj);
        }

        [HttpPost]
        public IActionResult MoneyTransfer(MyMoneyTransfer moneyTransfer)
        {
            try
            {
                //This is for validating the amount which is not greaterthan zero
                if (moneyTransfer.Amount <= 0)
                {
                    ModelState.AddModelError("Amount", "Please enter valid amount, which is not a negative number or greaterthan zero ");
                    return View();

                }
                // adding from user
                moneyTransfer.FromUser = HttpContext.Session.GetString("UserName");
                // transfer money to whom

                var toUser = _db.Users.Find(moneyTransfer.ToUser);
                if (toUser != null)
                {
                    var fromUserObjFromDatabase = _db.Users.Find(moneyTransfer.FromUser);
                    if (moneyTransfer.Amount > fromUserObjFromDatabase.AccountBalance)
                    {
                        ModelState.AddModelError("Amount", "You don't have sufficient balance,please enter the amount that is equal to your account balance");
                        return View();

                    }

                    toUser.AccountBalance = toUser.AccountBalance + moneyTransfer.Amount;

                    // transfer money from Whom
                    fromUserObjFromDatabase.AccountBalance = fromUserObjFromDatabase.AccountBalance - moneyTransfer.Amount;

                    _db.Users.Update(toUser);
                    _db.Users.Update(fromUserObjFromDatabase);


                    // for adding transaction data in datbase


                    MyTranscationHistory _transactionHistory = new MyTranscationHistory();
                    var rand = new Random();
                    _transactionHistory.TranscationId = rand.Next(100000, 1000000);
                    _transactionHistory.ToUsername = toUser.UserName;
                    _transactionHistory.FromUsername = fromUserObjFromDatabase.UserName;
                    _transactionHistory.DateTime = DateTime.Now;
                    _transactionHistory.Amount = moneyTransfer.Amount;

                    // For Ip Address 
                    //install using Microsoft.AspNetCore.HttpOverrides; version 2.2.0

                    IPAddress remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
                    string result = "";
                    if (remoteIpAddress != null)
                    {
                        // If we got an IPV6 address, then we need to ask the network for the IPV4 address 
                        // This usually only happens when the browser is on the same machine as the server.
                        if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                        {
                            remoteIpAddress = System.Net.Dns.GetHostEntry(remoteIpAddress).AddressList
                    .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                        }
                        result = remoteIpAddress.ToString();
                    }
                    _transactionHistory.IpAddress = result;
                    _db.TranscationHistory.Add(_transactionHistory);
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("ToUser", "User does not exist, please enter valid user");
                    return View();
                }

            }
            catch
            {
                return RedirectToAction("Oops", "Home");

            }

        }
        public IActionResult AddGiftCoupon()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddGiftCoupon(MyGiftVoucher myGiftVoucher)
        {
            try
            {
                var isVoucherValid = _db.GiftVoucher.Find(myGiftVoucher.VoucherId);
                if (isVoucherValid != null && isVoucherValid.Status == "Not Used")
                {
                    var user = _db.Users.Find(HttpContext.Session.GetString("UserName"));
                    user.AccountBalance = user.AccountBalance + isVoucherValid.Amount;
                    _db.Update(user);
                    _db.SaveChanges();


                    isVoucherValid.Status = "Used";
                    isVoucherValid.UsedBy = user.UserName;
                    isVoucherValid.UsedDatetime = DateTime.Now;
                    _db.Update(isVoucherValid);
                    _db.SaveChanges();
                    return RedirectToAction("Index", "Dashboard");


                }
                else
                {
                    ModelState.AddModelError("VoucherId", "Invalid voucher id, please enter valid voucher id");
                    return View();
                }


            }
            catch
            {
                return View();
            }



        }

        public IActionResult UserLoginHistory()
        {


            IEnumerable<MyLoginHistory> _loginHistory = _db.LoginHistory.AsNoTracking().Where(u => u.UserName.Contains(HttpContext.Session.GetString("UserName"))).ToList().OrderByDescending(u => u.UserName);
            return View(_loginHistory);
        }
        public IActionResult TranscationHistory()
        {


            IEnumerable<MyTranscationHistory> _TranscatioHistory = _db.TranscationHistory.Where(u => u.FromUsername.Contains(HttpContext.Session.GetString("UserName"))).OrderByDescending(u => u.DateTime);
            return View(_TranscatioHistory);
        }


        //User Loan Request
        public IActionResult LoanRequest()
        {
            return View();
        }

        [HttpPost]


        // Data insert into Database
        public IActionResult LoanRequest(MyLoanRequest myLoanRequest)
        {
            myLoanRequest.WhomRequested = HttpContext.Session.GetString("UserName");

            Random rand = new Random();
            myLoanRequest.LoanId = rand.Next() + myLoanRequest.LoanAmount;
            _db.LoanRequest.Add(myLoanRequest);
            _db.SaveChanges();
            return RedirectToAction("Index", "Dashboard");

        }

        public IActionResult LoanStatus()
        {
            IEnumerable<MyLoanRequest> request = _db.LoanRequest.Where(u => u.WhomRequested.Contains(HttpContext.Session.GetString("UserName"))).OrderByDescending(u => u.LastModifiedDate);
            return View(request);

        }


        public IActionResult MyLoanRequestHistory()
        {

            IEnumerable<MyLoanRequest> _myLoanRequests = _db.LoanRequest.Where(u => u.WhomRequested.Contains(HttpContext.Session.GetString("UserName"))).OrderByDescending(u => u.LastModifiedDate);
            return View(_myLoanRequests);

        }





    }


}






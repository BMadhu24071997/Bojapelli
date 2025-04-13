using Madhu.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace Madhu.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }


        public IActionResult SignIn()                                  //This is for creating signin page
        {
            try
            {
                if (HttpContext.Session.GetString("SignIn") == "True")    //If user is already signin then you no need to show sign in
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult SignIn(MySignIn obj)              //this is for db call
        {
            try
            {
                var isUserExists = _db.Users.Find(obj.UserName);
                if (isUserExists == null)                               //checking for user exists
                {
                    ModelState.AddModelError("UserName", "Username does not find, please enter correct Username");
                }
                else
                {
                    if (isUserExists.Password == obj.Password)            //If user exists then we going for password validation
                    {

                        HttpContext.Session.SetString("UserName", isUserExists.UserName);
                        HttpContext.Session.SetString("SignIn", "True");

                        if (isUserExists.EmailStatus == "Verified")
                        {
                            HttpContext.Session.SetString("EmailStatus", "True");
                        }
                        if (isUserExists.UserStatus == "Suspended")
                        {
                            HttpContext.Session.SetString("Suspended", "True");
                        }

                        if (HttpContext.Session.GetString("SignIn") == "True")
                        {

                            //login History 

                            MyLoginHistory myLoginHistory = new MyLoginHistory();

                            myLoginHistory.UserName = obj.UserName;
                            myLoginHistory.DateTime = DateTime.Now;
                            myLoginHistory.UserAction = "Sign In";




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

                            myLoginHistory.IpAddress = result;
                            _db.LoginHistory.Add(myLoginHistory);
                            _db.SaveChanges();

                        }

                        // User Admin 

                        if (isUserExists.Admin)
                        {

                            HttpContext.Session.SetString("Admin", "True");
                            return RedirectToAction("Admin", "Dashboard");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Dashboard");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Password does not match");
                    }
                }
                return View();
            }
            catch
            {
                return View();
            }
        }



        // this is for creating Registration page
        public IActionResult UserRegistration()

        {
            return View();
        }

        // This action method is for adding user to Database - DB call
        [HttpPost]
        public IActionResult UserRegistration(MyUsers obj)
        {
            try
            {
                var isUserExists = _db.Users.Find(obj.UserName);
                if (isUserExists == null)                               //checking for user exists
                {

                    _db.Users.Add(obj);
                    _db.SaveChanges();
                    return View("SignIn", "User");

                }
                else
                {
                    ModelState.AddModelError("UserName", "Username is already taken please try with  different username ");

                }
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        // Sign out the user
        public IActionResult SignOut()
        {
            try
            {
                var isUserExists = _db.Users.Find(HttpContext.Session.GetString("UserName"));

                HttpContext.Session.SetString("UserName", "");
                HttpContext.Session.SetString("SignIn", "False");
                HttpContext.Session.SetString("Suspended", "False");
                HttpContext.Session.SetString("EmailStatus", "False");
                if (HttpContext.Session.GetString("SignIn") == "False")
                {

                    MyLoginHistory myLoginHistory = new MyLoginHistory();

                    myLoginHistory.UserName = isUserExists.UserName;

                    myLoginHistory.DateTime = DateTime.Now;

                    myLoginHistory.UserAction = "Sign Out";


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

                    myLoginHistory.IpAddress = result;
                    _db.LoginHistory.Add(myLoginHistory);
                    _db.SaveChanges();

                }
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View();
            }


        }
        public IActionResult ForgotPassword()                                  //This is for creating Forgotpassword page
        {
            try
            {
                if (HttpContext.Session.GetString("SignIn") == "True")    //If user is already signin then you no need to show sign in
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        public IActionResult ForgotPassword(MySignIn signIn)                                  //This is for creating Forgotpassword page
        {
            try
            {
                var _userobj = _db.Users.Find(signIn.UserName);

                if (_userobj == null)
                {
                    ModelState.AddModelError("UserName", "User doesn't exist please enter valid user");
                    return View();
                }
                else
                {
                    ContactFormModel contactUs = new ContactFormModel();

                    contactUs.Email = "madhu.up.mb@gmail.com";
                    contactUs.Password = "shxwodfmjunnqjua"; // Email SMTP(simple Mail Transfer Protocol)
                    contactUs.Subject = "Your Password";
                    contactUs.ToEmail = _userobj.Email;
                    contactUs.Body = "Your password for username:" + _userobj.UserName + " is " + _userobj.Password;


                    using (MailMessage mm = new MailMessage(contactUs.Email, contactUs.ToEmail))
                    {
                        mm.Subject = contactUs.Subject;
                        mm.Body = contactUs.Body;
                        mm.IsBodyHtml = false;
                        using (SmtpClient smtp = new SmtpClient())
                        {
                            NetworkCredential NetworkCred = new NetworkCredential(contactUs.Email, contactUs.Password);
                            smtp.UseDefaultCredentials = false;
                            smtp.EnableSsl = true;
                            smtp.Host = "smtp.gmail.com";
                            smtp.Credentials = NetworkCred;
                            smtp.Port = 587;
                            smtp.Send(mm);
                            ViewBag.Message = "Email sent";
                        }
                    }

                    return View();

                }


                return View();
            }

            catch
            {
                return View();
            }
        }
        // verifying Email from Signin user
        public IActionResult VerifyEmail()
        {
            var _userobj = _db.Users.Find(HttpContext.Session.GetString("UserName"));

            EmailVerification emailVerification = new EmailVerification();

            emailVerification.Email = _userobj.Email;
            emailVerification.UserName = _userobj.UserName;

            return View(emailVerification);
        }

        //generating OTP for Email Verification 
        public IActionResult SendOTP(string? UserName)
        {
            var _userobj = _db.Users.Find(UserName);

            EmailVerification emailVerification = new EmailVerification();

            Random random = new Random();

            var _isOTPAlready = _db.VerifyEmail.Find(UserName);

            emailVerification.Email = _userobj.Email;
            emailVerification.UserName = _userobj.UserName;
            if (_isOTPAlready != null)
            {
                _isOTPAlready.OTP = random.Next(0, 1000000);
                _isOTPAlready.OTPGeneratedDateTime = DateTime.Now;
            }
            else
            {
                emailVerification.OTP = random.Next(0, 1000000);
                emailVerification.OTPGeneratedDateTime = DateTime.Now;
            }
           


            //sending email (OTP)

            ContactFormModel contactUs = new ContactFormModel();

            contactUs.Email = "madhu.up.mb@gmail.com";
            contactUs.Password = "shxwodfmjunnqjua"; // Email SMTP(simple Mail Transfer Protocol)
            contactUs.Subject = "Your Email verification OTP";
            contactUs.ToEmail = emailVerification.Email;

            if (_isOTPAlready != null)
            {
                contactUs.Body = "Your Email verification OTP is:" + _isOTPAlready.OTP;
            }
            else
            {
                contactUs.Body = "Your Email verification OTP is:" + emailVerification.OTP;
            }
            


            using (MailMessage mm = new MailMessage(contactUs.Email, contactUs.ToEmail))
            {
                mm.Subject = contactUs.Subject;
                mm.Body = contactUs.Body;
                mm.IsBodyHtml = false;
                using (SmtpClient smtp = new SmtpClient())
                {
                    NetworkCredential NetworkCred = new NetworkCredential(contactUs.Email, contactUs.Password);
                    smtp.UseDefaultCredentials = false;
                    smtp.EnableSsl = true;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(mm);
                    ViewBag.Message = "OTP is sent your Email";

                    HttpContext.Session.SetString("OTPGenerated", "True");

                    if(_isOTPAlready ==null)
                    {
                        _db.VerifyEmail.Add(emailVerification);
                    }
                    else
                    {
                        _db.VerifyEmail.Update(_isOTPAlready);
                    }
                    
                    _db.SaveChanges();


                    return RedirectToAction("VerifyEmail", "User");

                }

            }

        }

        [HttpPost]
        public IActionResult VerifyEmail(EmailVerification enteredDetails)
        {

            var _userobj = _db.Users.Find(HttpContext.Session.GetString("UserName"));
            var _isOTPAlready = _db.VerifyEmail.Find(_userobj.UserName);


            if (_isOTPAlready.OTP == enteredDetails.OTP)
            {
                var timeSpan = DateTime.Now - _isOTPAlready.OTPGeneratedDateTime;

                if (timeSpan.TotalSeconds > 180)
                {
                    ModelState.AddModelError("OTP", "OTP expired, please request new OTP, new OTP is valid for 3 minutes only");
                    return View();
                }

                _userobj.EmailStatus = "Verified";
                _db.Users.Update(_userobj);
                _db.SaveChanges();
                HttpContext.Session.SetString("EmailStatus", "True");
                ViewBag.messageOtp = "OTP Verified Sucessfully";

                if (HttpContext.Session.GetString("EmailStatus") == "True")
                {
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            else
            {
                ModelState.AddModelError("OTP", "OTP does not match, please enter correct OTP");
                return View();
            }

            return View();

        }

    }
}











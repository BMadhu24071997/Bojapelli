using Madhu.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
                        if(isUserExists.UserStatus == "Suspended")
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


    }


}





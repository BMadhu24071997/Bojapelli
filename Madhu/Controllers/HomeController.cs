using Madhu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;

namespace Madhu.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Oops()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }
        [HttpPost]

        public IActionResult ContactUs(ContactFormModel model)
        {

            if (model.Email.IsNullOrEmpty())
            {
                ModelState.AddModelError("Email", "Email Can't Be Empty");
                return View();

            }
            else
            {
                if (model.Mobile.ToString().IsNullOrEmpty())
                {
                    ModelState.AddModelError("Mobile", "Mobile Can't Be Empty");
                    return View();
                }
            }
            ContactFormModel contactUs = new ContactFormModel();

            contactUs.Email = "madhu.up.mb@gmail.com";
            contactUs.Password = "shxwodfmjunnqjua"; // Email SMTP(simple Mail Transfer Protocol) Password
            contactUs.Subject = model.Subject;
            contactUs.Mobile = model.Mobile;
            contactUs.ToEmail = "bmadhu2407@gmail.com";
            contactUs.Body = "Name : " + model.Name + "\n\n Email : " + model.Email + "\n\n Mobile No : " + model.Mobile + "\n\n Message : " + model.Body;


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

    }



}

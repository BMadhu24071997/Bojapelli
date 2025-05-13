using Madhu.Interfaces;
using Madhu.Models;
using Microsoft.AspNetCore.Mvc;

namespace Madhu.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ApplicationDbContext _db;

        public ProductController(IProductService productService, ApplicationDbContext db)
        {
            _productService = productService;
            _db = db;
        }

        public IActionResult Index()
        {
            var user = _db.Users.Find(HttpContext.Session.GetString("UserName"));
            //var cartItems = _db.Cart.Where(c => c.Username == user.UserName && c.Status == "Added To Cart").ToList();
           // ViewBag.TotalProductss = cartItems.Sum(c => c.Quantity);


            var products = _productService.GetProducts(); // Fetch product list
            return View(products);
        }

    }
}


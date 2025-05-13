using Humanizer;
using Madhu.Extensions;
using Madhu.Interfaces;
using Madhu.Models;
using Madhu.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Madhu.Controllers
{
    public class CartController : Controller
    {

        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _db;

        public CartController(IProductService productService, IUserService userService, ApplicationDbContext db)
        {
            _productService = productService;
            _userService = userService;
            _db = db;
        }

        public IActionResult Index()
        {

            string username = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("SignIn", "User");
            }

            var user = _userService.GetUser(username);
            if (user == null)
            {
                return RedirectToAction("Errors", "Home"); // Handle missing user
            }

            var _userAmount = _db.Users.Find(HttpContext.Session.GetString("UserName"));

            // creating session for account balance
            ViewBag.UserBalance = _userAmount.AccountBalance;

            // ✅ Fetch the cart from the database instead of session
            var cartItems = _db.Cart.Where(c => c.Username == username && c.Status == "Added To Cart").ToList();
            ViewData["TotalProducts"] = cartItems.Sum(c => c.Quantity);
            TempData["products"] = cartItems.Sum(c => c.Quantity);

            var cartCount = _db.Cart.Where(c => c.Username == username && c.Status == "Added To Cart").Sum(c => c.Quantity);
            if (cartCount > 0)
            {
                // Store the count in session
                HttpContext.Session.SetInt32("CartCount", cartCount);
            }
            else
            {
                HttpContext.Session.Remove("CartCount");
            }

            // ✅ Calculate the total cart value
            ViewBag.total = cartItems.Sum(item => item.Amount);

            return View(cartItems);
        }
        public IActionResult Buy(string sku)
        {

            try
            {
                string username = HttpContext.Session.GetString("UserName");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("SignIn", "User"); // Ensure user is logged in
                }

                var product = _productService.GetProduct(sku);
                if (product == null) return RedirectToAction("Errors", "Home");
                var cartItem = _db.Cart.FirstOrDefault(c => c.Username == username && c.ProductSku == sku && c.Status == "Added To Cart");



                if (cartItem != null)
                {
                    // Product already in cart, update quantity
                    cartItem.Quantity++;
                    cartItem.Amount = (int)(cartItem.Quantity * product.Price); // Update total amount
                }
                else
                {
                    // Always insert a new row instead of updating the existing one
                    _db.Cart.Add(new CartItem
                    {
                        Id = new Random().Next(0, 1000000), // Consider using a DB-generated ID
                        Username = username,
                        ProductSku = sku,
                        Quantity = 1, // Always add a new row with quantity 1
                        Amount = (int)product.Price, // Store the amount for this entry
                        Status = "Added To Cart"
                    });
                }

                _db.SaveChanges();
                return RedirectToAction("Index", "Cart");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Errors", "Home");
            }
        }
        public IActionResult AddToCart(string sku)
        {


            try
            {
                string username = HttpContext.Session.GetString("UserName");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("SignIn", "User"); // Ensure user is logged in
                }

                var product = _productService.GetProduct(sku);
                if (product == null) return RedirectToAction("Errors", "Home");
                var cartItem = _db.Cart.FirstOrDefault(c => c.Username == username && c.ProductSku == sku && c.Status == "Added To Cart");



                if (cartItem != null)
                {
                    // Product already in cart, update quantity
                    cartItem.Quantity++;
                    cartItem.Amount = (int)(cartItem.Quantity * product.Price); // Update total amount
                }
                else
                {
                    // Always insert a new row instead of updating the existing one
                    _db.Cart.Add(new CartItem
                    {
                        Id = new Random().Next(0, 1000000), // Consider using a DB-generated ID
                        Username = username,
                        ProductSku = sku,
                        Quantity = 1, // Always add a new row with quantity 1
                        Amount = (int)product.Price, // Store the amount for this entry
                        Status = "Added To Cart"
                    });
                }

                _db.SaveChanges();

                int cartCount = _db.Cart.Where(c => c.Username == username && c.Status == "Added To Cart").Sum(c => c.Quantity);
                HttpContext.Session.SetInt32("CartCount", cartCount);

                return RedirectToAction("Index", "Product");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Errors", "Home");
            }
        }


        public IActionResult Add(string sku)
        {

            var username = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "User"); // Redirect if user is not logged in
            }

            var product = _productService.GetProduct(sku);
            if (product == null) return RedirectToAction("Errors", "Home");
            var cartItem = _db.Cart.FirstOrDefault(c => c.Username == username && c.ProductSku == sku && c.Status == "Added To Cart");



            if (cartItem != null)
            {
                // Product already in cart, update quantity
                cartItem.Quantity++;
                cartItem.Amount = (int)(cartItem.Quantity * product.Price); // Update total amount
            }
            else
            {
                // Always insert a new row instead of updating the existing one
                _db.Cart.Add(new CartItem
                {
                    Id = new Random().Next(0, 1000000), // Consider using a DB-generated ID
                    Username = username,
                    ProductSku = sku,
                    Quantity = 1, // Always add a new row with quantity 1
                    Amount = (int)product.Price, // Store the amount for this entry
                    Status = "Added To Cart"
                });
            }

            _db.SaveChanges();
            return RedirectToAction("Index", "Cart");
        }

        public IActionResult Minus(string sku)
        {
            try
            {

                var username = HttpContext.Session.GetString("UserName");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login", "User"); // Redirect if user is not logged in
                }

                var cartItem = _db.Cart.FirstOrDefault(c => c.Username == username && c.ProductSku == sku && c.Status == "Added To Cart");


                if (cartItem != null)
                {
                    if (cartItem.Quantity > 1)
                    {
                        cartItem.Quantity--;
                        cartItem.Amount -= (int)_productService.GetProduct(sku).Price;
                    }
                    else
                    {
                        _db.Cart.Remove(cartItem); // Remove item if quantity reaches zero
                    }

                    _db.SaveChanges();
                }

                int cartCount = _db.Cart.Where(c => c.Username == username && c.Status == "Added To Cart").Sum(c => c.Quantity);
                if (cartCount > 0)
                {
                    HttpContext.Session.SetInt32("CartCount", cartCount);
                }
                else
                {
                    HttpContext.Session.Remove("CartCount");
                }
                return RedirectToAction("Index", "Cart");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Errors", "Home");
            }
        }

        public IActionResult Remove(string sku)
        {
            try
            {

                var username = HttpContext.Session.GetString("UserName");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login", "User"); // Redirect if user is not logged in
                }

                var cartItem = _db.Cart.FirstOrDefault(c => c.Username == username && c.ProductSku == sku && c.Status == "Added To Cart");

                if (cartItem != null)
                {
                    _db.Cart.Remove(cartItem); // Remove the product from the cart
                    _db.SaveChanges();
                }
                int cartCount = _db.Cart.Where(c => c.Username == username && c.Status == "Added To Cart").Sum(c => c.Quantity);
                if (cartCount > 0)
                {
                    HttpContext.Session.SetInt32("CartCount", cartCount);
                }
                else
                {
                    HttpContext.Session.Remove("CartCount");
                }

                return RedirectToAction("Index", "Cart");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Errors", "Home");
            }
        }



        public IActionResult AddShippingDetails()
        {
            ShippingDetails users = new ShippingDetails();
            var user = _db.Users.Find(HttpContext.Session.GetString("UserName"));
            users.Username = user.UserName;
            users.Mobile = user.Mobile;
            users.Email = user.Email;
            return View(users);
        }

        [HttpPost]
        public IActionResult AddShippingDetails(ShippingDetails shippingDetails)
        {
            var user = _db.Users.Find(HttpContext.Session.GetString("UserName"));
            shippingDetails.Username = user.UserName;
            shippingDetails.Mobile = user.Mobile;
            shippingDetails.Email = user.Email;



            if (shippingDetails.Username == null)
            {
                ModelState.AddModelError("Username", "Username can't be empty");
                return View();
            }
            else
            {
                if (shippingDetails.Mobile.ToString().IsNullOrEmpty())
                {
                    ModelState.AddModelError("Mobile", "Mobile can't be empty");
                    return View();
                }
                if (shippingDetails.Address.IsNullOrEmpty())
                {
                    ModelState.AddModelError("Address", "Address can't be empty, Enter correct address");
                    return View();
                }
                if (shippingDetails.Email.IsNullOrEmpty())
                {
                    ModelState.AddModelError("Email", "Email can't be empty");
                    return View();
                }

            }
            _db.shippingDetails.Add(shippingDetails);
            _db.SaveChanges();
            return RedirectToAction("PayAmount", "Cart");
        }


        public IActionResult PayAmount()
        {
            try
            {
                var user = _userService.GetUser(HttpContext.Session.GetString("UserName"));

                var _userAmount = _db.Users.Find(HttpContext.Session.GetString("UserName"));
                ViewBag.UserBalance = _userAmount.AccountBalance;
                var cartItems = _db.Cart.Where(c => c.Username == user.UserName && c.Status == "Added To Cart").ToList();
                ViewBag.TotalCartValue = cartItems.Sum(c => c.Amount);

                var cart = HttpContext.Session.Get<List<Product_Item>>("cart");
                if (cart != null)
                {
                    ViewBag.total = cart.Sum(s => s.Quantity * s.Product.Price);
                }
                else
                {
                    cart = new List<Product_Item>();
                    ViewBag.total = 0;
                }
                ViewBag.Cart = ViewBag.total;
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Errors", "Home");
            }

        }



        public IActionResult Pay(MyUsers _user)
        {
            try
            {
                var _userDetails = _db.Users.Find(HttpContext.Session.GetString("UserName"));

                Payment _payment = new Payment();
                Product _product = new Product();
                Product_Item _Item = new Product_Item();
                List<Product> products = new List<Product>();




                var cartItems = _db.Cart.Where(c => c.Username == _userDetails.UserName && c.Status == "Added To Cart").ToList();

                if (!cartItems.Any())
                {
                    return RedirectToAction("Index"); // No items to purchase
                }
                var amountt = cartItems.Sum(s => s.Amount);
                ViewBag.Amount = amountt;


                // Random number generator
                Random rand = new Random();
                _payment.PaymentId = rand.Next(0, 1000000000);
                TempData["OrderId"] = _payment.PaymentId;
                ViewData["data"] = _payment.PaymentId;

                _payment.Username = _userDetails.UserName;
                _payment.Amount = amountt;
                _payment.PaymentDate = DateTime.Now;


                _userDetails.AccountBalance = _userDetails.AccountBalance - _payment.Amount;
                _payment.OrderStatus = "Order Placed";

                // for track order
                Track _track = new Track();
                _track.Username = _userDetails.UserName;
                _track.OrderId = _payment.PaymentId;
                _track.Status = _payment.OrderStatus;
                _track.Date = DateTime.Now;

                //ViewBag.Message = _payment.PaymentId;
                _db.PaymentRecords.Add(_payment);
                _db.Users.Update(_userDetails);
                _db.trackOrder.Add(_track);

                _db.SaveChanges();
                return RedirectToAction("Success", "Cart");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Errors", "Home");
            }

        }
        public IActionResult Success()
        {
            try
            {
                var username = HttpContext.Session.GetString("UserName");

                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("SignIn", "User"); // Redirect if user is not logged in
                }

                var cartItems = _db.Cart.Where(c => c.Username == username && c.Status == "Added To Cart").ToList();

                if (cartItems.Any()) // Check if there are items to update
                {
                    foreach (var item in cartItems)
                    {
                        item.Status = "Purchased"; // Update status
                    }

                    _db.Cart.UpdateRange(cartItems); // Bulk update
                    _db.SaveChanges(); // Commit changes to database
                }
                int cartCount = _db.Cart.Where(c => c.Username == username && c.Status == "Added To Cart").Sum(c => c.Quantity);
                if (cartCount > 0)
                {
                    HttpContext.Session.SetInt32("CartCount", cartCount);
                }
                else
                {
                    HttpContext.Session.Remove("CartCount");
                }
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Errors", "Home");
            }
        }



        public IActionResult TrackOrder()
        {
            try
            {
                var username = HttpContext.Session.GetString("UserName");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("SignIn", "User"); // Redirect to login if session is empty
                }
                //IEnumerable<Payment> _payment = _db.PaymentRecords.Where(t => t.Username.Contains(HttpContext.Session.GetString("Username")));
                var payments = _db.PaymentRecords.Where(t => t.Username == username).OrderByDescending(t => t.PaymentDate).ToList();
                return View(payments);


            }
            catch (Exception ex)
            {
                return RedirectToAction("Errors", "Home");
            }
        }
        public IActionResult TrackAllOrder()
        {
            try
            {

                IEnumerable<Payment> _payment = _db.PaymentRecords;
                return View(_payment);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Errors", "Home");
            }
        }
        [HttpPost]
        public IActionResult UpdateOrderStatus(int PaymentId, string OrderStatus)
        {
            var order = _db.PaymentRecords.FirstOrDefault(p => p.PaymentId == PaymentId);
            if (order != null)
            {
                TrackAllOrders _trackorder = new TrackAllOrders();

                order.OrderStatus = OrderStatus;
                _trackorder.OrderStatus = order.OrderStatus;
                _trackorder.PaymentId = order.PaymentId;
                _trackorder.StatusUpdated = DateTime.Now;
                _trackorder.Amount = order.Amount;

                Track _track = new Track
                {
                    OrderId = order.PaymentId,   // Keeping same PaymentId
                    
                    Username = order.Username,   // Assuming Username exists in PaymentRecords
                    Status = OrderStatus,        // Updated Status
                    Date = DateTime.Now          // Capture current timestamp
                };

                _db.PaymentRecords.Update(order);
                _db.OrderStatus.Add(_trackorder);
                _db.trackOrder.Add(_track);
                _db.SaveChanges();
                return Ok(new { success = true });
            }
            return BadRequest(new { success = false });
        }
    }

}



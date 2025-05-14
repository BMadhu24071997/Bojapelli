using Madhu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Madhu.Controllers
{
    public class TrackController : Controller
    {


        private readonly ApplicationDbContext _db;
        public TrackController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(int? orderId)
        {
            try
            {
                if (!orderId.HasValue)
                {
                    return View(new List<Track>()); // Return empty list initially
                }


                IEnumerable<Track> _track = _db.trackOrder
                .AsNoTracking() // This disables change tracking and could resolve duplicate fetching
                .Where(t => t.OrderId.Equals(orderId))
                .OrderByDescending(t => t.Date)
                .ToList();



                if (!_track.Any())
                {
                    ViewBag.Message = "No tracking history found for this Order ID.";
                }
                ViewBag.id = orderId;

                return View(_track);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Errors", "Home");
            }
        }
    }
}

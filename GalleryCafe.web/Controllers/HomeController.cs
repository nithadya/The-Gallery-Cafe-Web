using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GalleryCafe.web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        public ActionResult Gallery()
        {
            return View();
        }


        // Mock Parking Availability (You can replace this with your own logic)
        public JsonResult GetParkingAvailability()
        {
            // Simulate random parking availability for the example
            bool isParkingAvailable = new Random().Next(0, 2) == 1;

            return Json(new
            {
                isAvailable = isParkingAvailable,
                message = isParkingAvailable ? "Parking is available!" : "Parking is full!"
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
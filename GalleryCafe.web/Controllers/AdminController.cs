using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GalleryCafe.web.Controllers
{
    public class AdminController : Controller
    {
        private Models.galleryCafe_dbEntities db = new Models.galleryCafe_dbEntities(); 


        // GET: Admin
        public ActionResult AdminHome()
        {
            return View();
        }
        public ActionResult AdminUsers()
        {
            return View();
        }
        public ActionResult AdminOrder()
        {
            return View();
        }
        public ActionResult AdminReservation()
        {
            return View();
        }
        public ActionResult AdminMenu()
        {
            return View();
        }
        public ActionResult AdminCreateMenu()
        {
            return View();
        }


        // API to get the dashboard statistics
        [HttpGet]
        public JsonResult GetStats()
        {
            var totalOrders = db.Orders.Count();
            var totalUsers = db.Users.Count();
            var totalReservations = db.Reservations.Count();

            return Json(new
            {
                totalOrders = totalOrders,
                totalUsers = totalUsers,
                totalReservations = totalReservations
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
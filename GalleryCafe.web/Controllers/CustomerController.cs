using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GalleryCafe.web.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult CustomerHome()
        {
            return View();
        }

        // GET: Customer
        public ActionResult CustomerProfile()
        {
            return View();
        }

        // GET: Customer
        public ActionResult CustomerOders()
        {
            return View();
        }

        // GET: Customer
        public ActionResult CustomerReservation()
        {
            return View();
        }
    }
}
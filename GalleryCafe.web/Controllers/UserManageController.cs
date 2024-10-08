using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GalleryCafe.web.Controllers
{
    public class UserManageController : Controller
    {
        // GET: UserManage
        public ActionResult Index()
        {
            return View();
        }


        private Models.galleryCafe_dbEntities db = new Models.galleryCafe_dbEntities();

        // Action to get user data from the database and return it in JSON format
        public JsonResult GetUsers()
        {
            // Query to get user data from the database
            var users = db.Users.Select(user => new
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                MobileNumber = user.MobileNumber,
                Gender = user.Gender,
                Role = user.Role,
                CreatedDate = user.CreatedDate
            }).ToList();

            // Return data as JSON
            return Json(new { data = users }, JsonRequestBehavior.AllowGet);
        }


        // Delete user action
        [HttpPost]
        public JsonResult DeleteUser(int userId)
        {
            var user = db.Users.Find(userId);
            if (user != null)
            {

                var reservations = db.Reservations.Where(r => r.UserId == userId).ToList();
                db.Reservations.RemoveRange(reservations);

                var orders = db.Orders.Where(r => r.UserId == userId).ToList();
                db.Reservations.RemoveRange(reservations);

                db.Users.Remove(user);
                db.SaveChanges();
                return Json(new { success = true, message = "User deleted successfully" });
            }
            return Json(new { success = false, message = "User not found" });
        }

        // Edit user action
        [HttpPost]
        public JsonResult EditUser(int userId, string firstName, string lastName, string email, string role)
        {
            var user = db.Users.Find(userId);
            if (user != null)
            {
                user.FirstName = firstName;
                user.LastName = lastName;
                user.Email = email;
                user.Role = role;


                db.SaveChanges();
                return Json(new { success = true, message = "User updated successfully" });
            }
            return Json(new { success = false, message = "User not found" });
        }
    }
}
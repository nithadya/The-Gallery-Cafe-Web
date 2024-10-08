using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GalleryCafe.web.Controllers
{
    public class OrderController : Controller
    {
        private Models.galleryCafe_dbEntities db = new Models.galleryCafe_dbEntities();

        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        // ************ USER FUNCTIONS ************

        // 1. Pre-order functionality (User-side)
        [HttpPost]
        public JsonResult PreOrder(int menuItemId, int quantity)
        {
            if (Session["UserId"] == null)
            {
                return Json(new { success = false, message = "Please log in to make a pre-order." });
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            var menuItem = db.Menus.Find(menuItemId); // Fetch the menu item to get its price

            if (menuItem == null)
            {
                return Json(new { success = false, message = "Menu item not found." });
            }

            var order = new Models.Order
            {
                UserId = userId,
                MenuItemId = menuItemId,
                Quantity = quantity,
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalPrice = quantity * menuItem.Price // Assuming Price is a property of MenuItem
            };

            db.Orders.Add(order);
            db.SaveChanges();

            return Json(new { success = true, message = "Order placed successfully!" });
        }





        // GET: Order Count
        public JsonResult GetOrderCount()
        {
            if (Session["UserId"] == null)
            {
                return Json(new { success = false, message = "User not logged in." }, JsonRequestBehavior.AllowGet);
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            // First, try to get the order count from the session
            if (Session["OrderCount"] != null)
            {
                int orderCountFromSession = (int)Session["OrderCount"];
                return Json(new { success = true, orderCount = orderCountFromSession }, JsonRequestBehavior.AllowGet);
            }

            // If it's not in the session, calculate it from the database
            var orderCountFromDb = db.Orders.Count(o => o.UserId == userId);

            // Store the order count in the session
            Session["OrderCount"] = orderCountFromDb;

            return Json(new { success = true, orderCount = orderCountFromDb }, JsonRequestBehavior.AllowGet);
        }





        // GET: Orders for customer
        public JsonResult GetCustomerOrders()
        {
            if (Session["UserId"] == null)
            {
                return Json(new { success = false, message = "You need to log in." });
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            var orders = db.Orders.Where(o => o.UserId == userId)
                                  .Select(o => new
                                  {
                                      o.OrderId,
                                      o.Menu.Name,
                                      o.Quantity,
                                      o.OrderDate,
                                      o.Status,
                                      TotalPrice = o.TotalPrice // Include TotalPrice in the response
                                  }).ToList();

            return Json(new { success = true, data = orders }, JsonRequestBehavior.AllowGet);
        }





        // 2. Edit an order (User-side)
        [HttpPost]
        public JsonResult EditOrder(int orderId, int quantity)
        {
            var order = db.Orders.Find(orderId);
            if (order == null || order.Status != "Pending")
            {
                return Json(new { success = false, message = "Order not found or already processed." });
            }

            var menuItem = db.Menus.Find(order.MenuItemId); // Fetch the menu item to get its price

            if (menuItem == null)
            {
                return Json(new { success = false, message = "Menu item not found." });
            }

            order.Quantity = quantity;
            order.TotalPrice = quantity * menuItem.Price; // Update total price

            db.SaveChanges();

            return Json(new { success = true, message = "Order updated successfully." });
        }





        // 3. Delete an order (User-side)
        [HttpPost]
        public JsonResult DeleteOrder(int orderId)
        {
            var order = db.Orders.Find(orderId);
            if (order == null || order.Status != "Pending")
            {
                return Json(new { success = false, message = "Order not found or cannot be deleted." });
            }

            db.Orders.Remove(order);
            db.SaveChanges();

            return Json(new { success = true, message = "Order deleted successfully." });
        }






        // ************ ADMIN FUNCTIONS ************

        // 1. Cancel an order (Admin-side)
        [HttpPost]
        public JsonResult CancelOrder(int orderId)
        {
            var order = db.Orders.Find(orderId);
            if (order == null || order.Status == "Cancelled")
            {
                return Json(new { success = false, message = "Order not found or already cancelled." });
            }

            order.Status = "Cancelled";
            db.SaveChanges();

            return Json(new { success = true, message = "Order cancelled successfully." });
        }




        // 2. Delete an order (Admin-side)
        [HttpPost]
        public JsonResult DeleteOrderAdmin(int orderId)
        {
            var order = db.Orders.Find(orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }

            db.Orders.Remove(order);
            db.SaveChanges();

            return Json(new { success = true, message = "Order deleted successfully by admin." });
        }





        // 3. Update an order (Admin-side)
        [HttpPost]
        public JsonResult UpdateOrder(int orderId, int menuItemId, int quantity, string status)
        {
            var order = db.Orders.Find(orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }

            order.MenuItemId = menuItemId;
            order.Quantity = quantity;
            order.Status = status;

            var menuItem = db.Menus.Find(menuItemId); // Fetch the menu item to get its price
            if (menuItem != null)
            {
                order.TotalPrice = quantity * menuItem.Price; // Update total price
            }

            db.SaveChanges();

            return Json(new { success = true, message = "Order updated successfully." });
        }




        // GET: All orders for admin
        public JsonResult GetAllOrders()
        {
            var orders = db.Orders.Select(o => new
            {
                o.OrderId,
                o.Menu.Name,
                o.User.FirstName,
                o.Quantity,
                o.OrderDate,
                o.Status,
                TotalPrice = o.TotalPrice // Include TotalPrice in the response
            }).ToList();

            return Json(new { success = true, data = orders }, JsonRequestBehavior.AllowGet);
        }





        // 4. Approve an order (Admin-side)
        [HttpPost]
        public JsonResult ApproveOrder(int orderId)
        {
            var order = db.Orders.Find(orderId);
            if (order == null || order.Status != "Pending")
            {
                return Json(new { success = false, message = "Order not found or already processed." });
            }

            order.Status = "Approved";
            db.SaveChanges();

            return Json(new { success = true, message = "Order approved successfully." });
        }

    }
}
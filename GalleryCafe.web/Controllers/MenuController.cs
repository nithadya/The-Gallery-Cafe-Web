using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GalleryCafe.web.Controllers
{
    public class MenuController : Controller
    {
        private Models.galleryCafe_dbEntities db = new Models.galleryCafe_dbEntities();
        // GET: Menu
        public ActionResult Menu()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }


        // ************ USER FUNCTIONS ************

        // 1.Retrieve menu items by cuisine type (User-side)
        public JsonResult GetMenuByCuisine(string cuisineType)
        {
            var menuItems = db.Menus
                              .Where(m => m.CuisineType == cuisineType && m.Available == true)
                              .Select(m => new
                              {
                                  m.MenuItemId,
                                  m.Name,
                                  m.Description,
                                  m.Price,
                                  ImagePath = "/Content/img/" + m.MenuItemId + ".png" // Image path
                              })
                              .ToList();

            return Json(new { success = true, data = menuItems }, JsonRequestBehavior.AllowGet);
        }






        // ************ ADMIN FUNCTIONS ************

        // 1. Add a new menu item (Admin-side)
        [HttpPost]
        public JsonResult AddMenuItem(string name, string description, decimal price, string cuisineType, bool available)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(name) || price < 0)
            {
                return Json(new { success = false, message = "Invalid input data." });
            }

            try
            {
                var menuItem = new Models.Menu
                {
                    Name = name,
                    Description = description,
                    Price = price,
                    CuisineType = cuisineType,
                    Available = available
                };

                db.Menus.Add(menuItem);
                db.SaveChanges();

                return Json(new { success = true, message = "Menu item added successfully!" });
            }
            catch (Exception ex)
            {
                // Log the exception (implement your logging mechanism here)
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }






        // 2. Update an existing menu item (Admin-side)
        [HttpPost]
        public JsonResult UpdateMenuItem(int menuItemId, string name, string description, decimal price, string cuisineType, bool available)
        {
            try
            {
                // Find the menu item by ID
                var menuItem = db.Menus.Find(menuItemId);
                if (menuItem == null)
                {
                    return Json(new { success = false, message = "Menu item not found." });
                }

                // Update the menu item properties
                menuItem.Name = name;
                menuItem.Description = description;
                menuItem.Price = price;
                menuItem.CuisineType = cuisineType;
                menuItem.Available = available;

                // Save changes to the database
                db.SaveChanges();

                // Return success response
                return Json(new { success = true, message = "Menu item updated successfully!" });
            }
            catch (Exception ex)
            {

                // Return error response with exception message
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }






        // 3. Delete a menu item (Admin-side)
        [HttpPost]
        public JsonResult DeleteMenuItem(int menuItemId)
        {
            // Find the menu item by ID
            var menuItem = db.Menus.Find(menuItemId);
            if (menuItem == null)
            {
                return Json(new { success = false, message = "Menu item not found." });
            }
            // Find associated orders for the menu item
            var relatedOrders = db.Orders.Where(o => o.MenuItemId == menuItemId).ToList();
            // Delete associated orders from the orders table
            if (relatedOrders.Any())
            {
                db.Orders.RemoveRange(relatedOrders);
            }
            // Delete the menu item from the menu table
            db.Menus.Remove(menuItem);
            // Save changes to the database
            db.SaveChanges();
            return Json(new { success = true, message = "Menu item and related orders deleted successfully!" });
        }






        // 4. Retrieve all menu items (Admin-side)
        public JsonResult GetAllMenuItems()
        {
            var menuItems = db.Menus.Select(m => new
            {
                m.MenuItemId,
                m.Name,
                m.Description,
                m.Price,
                m.CuisineType,
                m.Available
            }).ToList();

            return Json(new { success = true, data = menuItems }, JsonRequestBehavior.AllowGet);
        }


        // Search for food items by name or cuisine
        public JsonResult SearchFood(string query)
        {
            var foodItems = db.Menus
                              .Where(m => m.Name.Contains(query) || m.CuisineType.Contains(query))
                              .Select(m => new
                              {
                                  m.MenuItemId,
                                  m.Name,
                                  m.Description,
                                  m.Price,
                                  ImagePath = "/Content/img/" + m.MenuItemId + ".png"
                              })
                              .ToList();

            if (foodItems.Any())
            {
                return Json(new { success = true, data = foodItems }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No results found." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
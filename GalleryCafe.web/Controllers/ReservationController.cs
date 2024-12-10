using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GalleryCafe.web.Controllers
{
    public class ReservationController : Controller
    {

        private Models.galleryCafe_dbEntities db = new Models.galleryCafe_dbEntities();

        public ActionResult Booking()
        {
            return View();
        }

        // Fetch all available and booked tables
        public JsonResult GetAvailableTables()
        {
            // Example: Total tables range from 101 to 120
            var tables = Enumerable.Range(101, 20).ToList();
            var bookedTables = db.Reservations
                                 .Where(r => r.Status == "Approved" || r.Status == "Pending")
                                 .Select(r => r.TableNumber)
                                 .ToList();

            return Json(new { tables = tables, bookedTables = bookedTables }, JsonRequestBehavior.AllowGet);
        }


        // Get reservations for a logged-in customer
        public JsonResult GetCustomerReservations()
        {
            if (Session["UserId"] == null)
            {
                return Json(new { success = false, message = "Please log in to view reservations." }, JsonRequestBehavior.AllowGet);
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            try
            {
                var reservations = db.Reservations
                    .Where(r => r.UserId == userId)
                    .Select(r => new
                    {
                        r.ReservationId,
                        r.TableNumber,
                        r.ReservationDate,  
                        r.NumberOfGuests,
                        r.Status
                    });

                return Json(new { success = true, data = reservations.ToList() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
               
                return Json(new { success = false, message = "An error occurred while retrieving reservations.", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }




        // Admin view - Get all reservations
        public JsonResult GetAllReservations()
        {
            var reservations = db.Reservations.Select(r => new
            {
                r.ReservationId,
                r.TableNumber,
                r.ReservationDate,
                r.NumberOfGuests,
                r.Status,
                UserName = r.User.FirstName + " " + r.User.LastName
            }).ToList();

            return Json(new { data = reservations }, JsonRequestBehavior.AllowGet);
        }




        // Create reservation (AJAX call)
        [HttpPost]
        public JsonResult CreateReservation(Models.Reservation model)
        {
            if (Session["UserId"] == null)
            {
                return Json(new { success = false, message = "Please log in to make a reservation." });
            }

            if (ModelState.IsValid)
            {
                model.UserId = Convert.ToInt32(Session["UserId"]);
                model.CreatedDate = DateTime.Now;
                model.Status = "Pending";
                db.Reservations.Add(model);
                db.SaveChanges();

                return Json(new { success = true, message = "Reservation successful!" });
            }

            return Json(new { success = false, message = "Invalid reservation details." });
        }





        // Update reservation status (Admin)
        [HttpPost]
        public JsonResult UpdateReservationStatus(int reservationId, string status)
        {
            var reservation = db.Reservations.Find(reservationId);
            if (reservation != null)
            {
                reservation.Status = status;
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }



        // Update a reservation
        [HttpPost]
        public JsonResult UpdateReservation(int reservationId, int tableNumber, DateTime reservationDate, int numberOfGuests, string status)
        {
            try
            {
                var reservation = db.Reservations.Find(reservationId);
                if (reservation == null)
                {
                    return Json(new { success = false, message = "Reservation not found." });
                }

                reservation.TableNumber = tableNumber;
                reservation.ReservationDate = reservationDate;
                reservation.NumberOfGuests = numberOfGuests;
                reservation.Status = status;

                db.SaveChanges();

                return Json(new { success = true, message = "Reservation updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred. Please try again." });
            }
        }


        // Delete a reservation
        [HttpPost]
        public JsonResult DeleteReservation(int reservationId)
        {
            try
            {
                var reservation = db.Reservations.Find(reservationId);
                if (reservation == null)
                {
                    return Json(new { success = false, message = "Reservation not found." });
                }

                db.Reservations.Remove(reservation);
                db.SaveChanges();

                return Json(new { success = true, message = "Reservation deleted successfully." });
            }

            catch (Exception ex)
            {
                // Log the exception (ex) for debugging
                return Json(new { success = false, message = "An unexpected error occurred. Please try again." });
            }
        }
    }
}
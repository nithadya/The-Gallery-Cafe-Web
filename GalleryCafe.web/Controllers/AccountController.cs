using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity.Validation;
using System.Text.RegularExpressions;


namespace GalleryCafe.web.Controllers
{
    public class AccountController : Controller
    {
        private Models.galleryCafe_dbEntities db = new Models.galleryCafe_dbEntities();

        // GET: Register User
        public ActionResult Register()
        {
            return View();
        }

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        // GET: Register Admin
        public ActionResult RegisterAdmin()
        {
            return View();
        }

        // GET: Login
        public ActionResult AdminLogin()
        {
            return View();
        }



        //1.Action to handle user registration with validation
        [HttpPost]
        public JsonResult RegisterUser(Models.User user)
        {
            // Email validation
            if (!IsValidEmail(user.Email))
            {
                return Json(new { success = false, message = "Invalid email format." });
            }

            // Password validation (min 6 characters)
            if (string.IsNullOrWhiteSpace(user.PasswordHash) || user.PasswordHash.Length < 6)
            {
                return Json(new { success = false, message = "Password must be at least 6 characters long." });
            }

            // Check if email is already registered
            var existingUser = db.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
            {
                return Json(new { success = false, message = "Email is already registered." });
            }


            // Register new user with default role as "Customer"
            var newUser = new Models.User()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                MobileNumber = user.MobileNumber,
                Gender = user.Gender,
                PasswordHash = FormsAuthentication.HashPasswordForStoringInConfigFile(user.PasswordHash, "SHA1"), // Hash the password
                Role = "Customer", // Default role
                CreatedDate = DateTime.Now
            };

            // Add new user to the database
            db.Users.Add(newUser);
            db.SaveChanges();

            return Json(new { success = true, message = "Registration successful!" });
        }







        // 2. Action to handle admin registration with validation

        [HttpPost]
        public JsonResult RegisterAdminUser(Models.User user)
        {
            try
            {
                // Email validation
                if (!IsValidEmail(user.Email))
                {
                    return Json(new { success = false, message = "Email is required and must be in a valid format." });
                }

                // Password validation (min 6 characters)
                if (string.IsNullOrWhiteSpace(user.PasswordHash) || user.PasswordHash.Length < 6)
                {
                    return Json(new { success = false, message = "Password must be at least 6 characters long." });
                }

                // Check if email is already registered
                var existingUser = db.Users.FirstOrDefault(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    return Json(new { success = false, message = "Email is already registered." });
                }

                // Register new user with role as "Admin"
                var newAdminUser = new Models.User()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    MobileNumber = user.MobileNumber,
                    Gender = user.Gender,
                    PasswordHash = FormsAuthentication.HashPasswordForStoringInConfigFile(user.PasswordHash, "SHA1"), // Hash the password
                    Role = user.Role, // Set role as Admin
                    CreatedDate = DateTime.Now
                };

                // Add new admin user to the database
                db.Users.Add(newAdminUser);
                db.SaveChanges();

                return Json(new { success = true, message = "Admin registration successful!" });
            }
            catch (DbEntityValidationException ex)
            {
                // Handle validation errors and log them for debugging
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }
                return Json(new { success = false, message = "There was an error during registration. Please check the input values." });
            }
            catch (Exception ex)
            {
                // Log any other exceptions
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }






        //3. Action to handle user login
        [HttpPost]
        public JsonResult LoginUser(string email, string password)
        {
            try
            {
                // Validate the email
                if (!IsValidEmail(email))
                {
                    return Json(new { success = false, message = "Email is required and must be in a valid format." });
                }

                // Password cannot be empty
                if (string.IsNullOrWhiteSpace(password))
                {
                    return Json(new { success = false, message = "Password is required." });
                }

                string hashedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
                var user = db.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == hashedPassword);

                if (user != null)
                {
                    Session["UserId"] = user.UserId;
                    Session["Username"] = user.FirstName;
                    Session["Role"] = user.Role; // Store user role in session

                    // Redirect based on user role
                    if (user.Role == "Admin")
                    {
                        return Json(new { success = true, redirectUrl = Url.Action("AdminHome", "Admin") });
                    }
                    else if (user.Role == "Staff")
                    {
                        return Json(new { success = true, redirectUrl = Url.Action("StaffHome", "Staff") });
                    }
                    else if (user.Role == "Customer")
                    {
                        return Json(new { success = true, redirectUrl = Url.Action("index", "Home") });
                    }
                }
                return Json(new { success = false, message = "Invalid email or password." });
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { success = false, message = "An error occurred during login. Please try again." });
            }
        }



        //4. Action to handle user logout
        [HttpPost]
        public JsonResult Logout()
        {
            try
            {
                Session.Clear(); // Clear the session on logout
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { success = false, message = "An error occurred during logout. Please try again." });
            }
        }





        //5. Action to get current logged-in user
        public JsonResult GetCurrentUser()
        {
            if (Session["UserId"] != null)
            {
                var userInfo = new
                {
                    userId = Session["UserId"],
                    username = Session["Username"],
                    role = Session["Role"] // Get the role of the user
                };

                return Json(new { success = true, data = userInfo }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = "No user logged in." }, JsonRequestBehavior.AllowGet);
        }





        //6. Email validation method
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false; // Invalid if email is empty or whitespace
            }

            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }


        // get profile data
        [HttpGet]
        public JsonResult GetUserProfile()
        {
            if (Session["UserId"] != null)
            {
                var userId = (int)Session["UserId"];
                var user = db.Users.FirstOrDefault(u => u.UserId == userId);

                if (user != null)
                {
                    var userProfile = new
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        MobileNumber = user.MobileNumber,
                        Gender = user.Gender,
                        Role = user.Role,
                       // ProfilePictureUrl = user.ProfilePictureUrl ?? "/images/default-avatar.png"
                    };

                    return Json(new { success = true, data = userProfile }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { success = false, message = "No user logged in." }, JsonRequestBehavior.AllowGet);
        }

    }
}
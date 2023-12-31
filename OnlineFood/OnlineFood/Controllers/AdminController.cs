using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Mvc;
using OnlineFood;  // Make sure to include the namespace of your models

namespace OnlineFood.Controllers
{
    public class AdminController : Controller
    {
        private OnlineFoodDeliveryAPPDBEntities1 dbContext = new OnlineFoodDeliveryAPPDBEntities1();
        // private OnlineFoodDeliveryAPPDBEntities db = new OnlineFoodDeliveryAPPDBEntities();  // Replace with your actual DbContext class name

        // GET: Admin
        public ActionResult Index()
        {
            // You can customize this method based on your requirements
            // For example, you can retrieve the restaurants associated with the logged-in admin
            // and pass them to the view.
            // Here, I'm just returning all restaurants as an example.

            var restaurants = dbContext.Restaurants.ToList();
            return View(restaurants);
        }

        // GET: Admin/Create
        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var adminId = GetLoggedInAdminId(); // You need to implement this method
                    restaurant.RestaurentAdminId = adminId;

                    dbContext.Restaurants.Add(restaurant);
                    dbContext.SaveChanges();
                    TempData["Message"] = "Restaurant created successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Log or handle the exception as needed
                    Console.WriteLine(ex.Message);
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the restaurant.");
                }
            }

            return View(restaurant);
        }


        // GET: Admin/Edit/5
        public ActionResult Edit(int id)
        {
            var restaurant = dbContext.Restaurants.Find(id);
            if (IsAuthorizedToEdit(restaurant))
            {
                return View(restaurant);
            }

            return RedirectToAction("Index");
        }

        
        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Restaurant updatedRestaurant)
        {
            if (ModelState.IsValid)
            {
                var existingRestaurant = dbContext.Restaurants.Find(updatedRestaurant.RestaurentId);
                if (IsAuthorizedToEdit(existingRestaurant))
                {
                    existingRestaurant.RestaurentName = updatedRestaurant.RestaurentName;
                    existingRestaurant.RestaurentLogoUrl = updatedRestaurant.RestaurentLogoUrl;
                    existingRestaurant.RestaurentAvailable = updatedRestaurant.RestaurentAvailable;

                    try
                    {
                        dbContext.SaveChanges();
                        TempData["Message"] = "Restaurant updated successfully!";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        // Log or handle the exception as needed
                        Console.WriteLine(ex.Message);
                        ModelState.AddModelError(string.Empty, "An error occurred while saving changes.");
                        return View("Edit", updatedRestaurant); // Return to the Edit view with the updated model
                    }
                }
                else
                {
                    TempData["Message"] = "You are not authorized to edit this restaurant.";
                }
            }

            // If ModelState is not valid or there was an error, return to the Edit view
            return View("Edit", updatedRestaurant);
        }



        // Helper method to check if the logged-in admin is authorized to edit the restaurant
        private bool IsAuthorizedToEdit(Restaurant restaurant)
        {
            if (restaurant != null && (long)Session["AdminId"] == restaurant.RestaurentAdminId)
            {
                return true;
            }

            return false;
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Restaurant restaurant = dbContext.Restaurants.Find(id);

            if (restaurant == null)
            {
                return HttpNotFound();
            }

            return View(restaurant);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Restaurant restaurant = dbContext.Restaurants.Find(id);
            dbContext.Restaurants.Remove(restaurant);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbContext.Dispose();
            }

            base.Dispose(disposing);
        }

        private long GetLoggedInAdminId()
        {
            // Replace this with your actual logic to get the admin ID from the user's identity or session
            var adminIdClaim = ((ClaimsIdentity)User.Identity).FindFirst("AdminId");

            if (adminIdClaim != null && long.TryParse(adminIdClaim.Value, out var adminId))
            {
                return adminId;
            }

            // Return a default value or handle the case when the admin ID is not available
            return 0;
        }


    }
}



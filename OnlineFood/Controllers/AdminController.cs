using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Mvc;
using OnlineFood.Models;  // Make sure to include the namespace of your models

namespace OnlineFood.Controllers
{
    public class AdminController : Controller
    {
        private long GetLoggedInAdminId()
        {
            return Session["AdminId"] as long? ?? 0;
        }

        private OnlineFoodDeliveryAPPDBEntities1 dbContext = new OnlineFoodDeliveryAPPDBEntities1();

        // GET: Admin
        // GET: Admin
        public ActionResult Index()
        {
            // Get the logged-in admin ID
            var adminId = GetLoggedInAdminId();

            // Retrieve restaurants associated with the logged-in admin
            var restaurants = dbContext.Restaurants.Where(r => r.RestaurentAdminId == adminId).ToList();

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
            var adminId = GetLoggedInAdminId(); // You need to implement this method
            restaurant.RestaurentAdminId = adminId;


            dbContext.Restaurants.Add(restaurant);
            dbContext.SaveChanges();
            TempData["Message"] = "Restaurant created successfully!";
            return RedirectToAction("Index");



        }

        private bool IsAuthorizedToEdit(Restaurant restaurant)
        {
            var adminId = GetLoggedInAdminId();
            return restaurant != null && adminId == restaurant.RestaurentAdminId;
        }

        // Use [HttpGet] attribute to specify the action for GET requests
        [HttpGet]
        public ActionResult EditRestaurent(long? id)
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

            if (!IsAuthorizedToEdit(restaurant))
            {
                TempData["Message"] = "You are not authorized to edit this restaurant.";
                return RedirectToAction("Index");
            }

            ViewBag.Restaurants = new SelectList(dbContext.Restaurants, "RestaurentId", "RestaurentName");

            // Pass only the necessary data to the view (e.g., the restaurant ID and name)
            var model = new RestaurantEditViewModel
            {
                RestaurentId = restaurant.RestaurentId,
                RestaurentName = restaurant.RestaurentName,
                // Add other properties as needed
            };

            return View("Edit", model);

           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRestaurent(Restaurant updatedRestaurant)
        {
            if (ModelState.IsValid)
            {
                var existingRestaurant = dbContext.Restaurants.Find(updatedRestaurant.RestaurentId);

                if (existingRestaurant == null)
                {
                    return HttpNotFound();
                }

                if (IsAuthorizedToEdit(existingRestaurant))
                {
                    try
                    {
                        existingRestaurant.RestaurentName = updatedRestaurant.RestaurentName;
                        existingRestaurant.RestaurentLogoUrl = updatedRestaurant.RestaurentLogoUrl;
                        existingRestaurant.RestaurentAvailable = updatedRestaurant.RestaurentAvailable;

                        dbContext.Entry(existingRestaurant).State = EntityState.Modified;
                        dbContext.SaveChanges();

                        TempData["Message"] = "Restaurant updated successfully!";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        ModelState.AddModelError(string.Empty, "An error occurred while saving changes.");
                        return View(updatedRestaurant);
                    }
                }
                else
                {
                    TempData["Message"] = "You are not authorized to edit this restaurant.";
                }
            }

            return View(updatedRestaurant);
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

            if (IsAuthorizedToEdit(restaurant))
            {
                dbContext.Restaurants.Remove(restaurant);
                dbContext.SaveChanges();
                TempData["Message"] = "Restaurant deleted successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Message"] = "You are not authorized to delete this restaurant.";
                return RedirectToAction("Index");
            }
        }



        // GET: Admin/Details/5
        public ActionResult Details(long? id)
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

        public ActionResult ShowFoodItems(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Retrieve the selected menu and its associated food items
            var menu = dbContext.Restaurants.Find(id);
            var foodItems = dbContext.Foods.Where(f => f.FoodRestaurentId == id).ToList();

            if (menu == null)
            {
                return HttpNotFound();
            }

            // Pass the menu, food items, and restaurant name to the view
            ViewBag.Menu = menu;
            ViewBag.RestaurantName = menu.RestaurentName;

            return View(foodItems);
        }


        // Details Action
        public ActionResult DetailsShowFoodItem(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var food = dbContext.Foods.Find(id);

            if (food == null)
            {
                return HttpNotFound();
            }

            return View(food);
        }

        // Create Action
        [HttpGet]
        public ActionResult CreateShowFoodItem(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Retrieve the list of restaurants for the dropdown
            var restaurants = dbContext.Restaurants.Select(r => new SelectListItem
            {
                Value = r.RestaurentId.ToString(),
                Text = r.RestaurentName
            });

            // Pass the restaurant information and the dropdown list to the view
            ViewBag.RestaurantId = id;
            ViewBag.Restaurants = restaurants;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateShowFoodItem(Food food)
        {
            var selectedRestaurantId = food.FoodRestaurentId;

            if (ModelState.IsValid)
            {
                dbContext.Foods.Add(food);
                dbContext.SaveChanges();

                // Redirect to the ShowFoodItems action of the associated restaurant
                return RedirectToAction("ShowFoodItems", new { id = food.FoodRestaurentId });
            }

            // If the model state is not valid, reload the restaurant information and return to the Create view with the current model
            ViewBag.RestaurantId = food.FoodRestaurentId;
            ViewBag.RestaurantName = dbContext.Restaurants.Find(food.FoodRestaurentId)?.RestaurentName;

            return View(food);
        }

        // Edit Action
        [HttpGet]
        public ActionResult EditShowFoodItem(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var food = dbContext.Foods.Find(id);

            if (food == null)
            {
                return HttpNotFound();
            }

            // Retrieve the list of restaurants for the dropdown
            var restaurants = dbContext.Restaurants.Select(r => new SelectListItem
            {
                Value = r.RestaurentId.ToString(),
                Text = r.RestaurentName
            });

            // Set the ViewData for the dropdown
            ViewBag.Restaurants = restaurants;

            return View(food);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditShowFoodItem(Food food)
        {
            if (ModelState.IsValid)
            {
                dbContext.Entry(food).State = EntityState.Modified;
                dbContext.SaveChanges();

                return RedirectToAction("ShowFoodItems", new { id = food.FoodRestaurentId });
            }

            // If the model state is not valid, return to the Edit view with the current model
            return View(food);
        }

        // Delete Action
        [HttpGet]
        public ActionResult DeleteShowFoodItem(long id)
        {
            var food = dbContext.Foods.Find(id);

            if (food == null)
            {
                return HttpNotFound();
            }

            return View(food);
        }

        [HttpPost, ActionName("DeleteShowFoodItem")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedShowFoodItem(long id)
        {
            var food = dbContext.Foods.Find(id);

            if (food == null)
            {
                return HttpNotFound();
            }

            dbContext.Foods.Remove(food);
            dbContext.SaveChanges();

            // Redirect to the ShowFoodItems action
            return RedirectToAction("ShowFoodItems", new { id = food.FoodRestaurentId });
        }


    }
}
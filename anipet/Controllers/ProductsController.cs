using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using anipet.Models;

namespace anipet.Controllers
{
    public class ProductsController : Controller
    {
        private DBContext db = new DBContext();

        public class CreateProductsViewModel
        {
            public CreateProductsViewModel()
            {
                // Init the array in case there are no stores selected
                StoresIds = new List<int>();
            }

            public Product Product { get; set; }

            public int FoodSourceId { get; set; }

            public List<int> StoresIds { get; set; }
        }

        // GET: Products
        public ActionResult Index()
        {
            var currentUser = (User)HttpContext.Session["user"];

            if (currentUser == null)
            {
                return RedirectToAction("Index", "Error");
            }

            // Reselect the user from the DB - in case the favorite product was changed since the session started
            currentUser = db.Users.Where(r => r.Id == currentUser.Id).Include(r => r.FavoriteProduct).FirstOrDefault();
            ViewBag.FavoriteProduct = currentUser.FavoriteProduct;
            ViewBag.StoresList = db.Stores.Select(r => r.City);
            ViewBag.FoodSourceList = db.FoodSources.Select(r => r.Name);
            return View(db.Products.Include(r => r.FoodSource).Include(r => r.Stores).ToList());
        }

        // GET: Products/Search
        public ActionResult Search(string name = null, string material = null, string city = null, int price = 0)
        {
            var currentUser = (User)HttpContext.Session["user"];
            ViewBag.FavoriteProduct = currentUser.FavoriteProduct;
            ViewBag.StoresList = db.Stores.Select(r => r.City);
            ViewBag.FoodSourceList = db.FoodSources.Select(r => r.Name);

            if (currentUser == null)
            {
                return RedirectToAction("Index", "Error");
            }

            var returnDataQurey = db.Products.Include(r => r.FoodSource).Include(r => r.Stores);

            if (!string.IsNullOrEmpty(name))
            {
                returnDataQurey = returnDataQurey.Where(r => r.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(city))
            {
                returnDataQurey = returnDataQurey.Where(r => r.Stores.Any(i => i.City == city));
            }

            if (!string.IsNullOrEmpty(material))
            {
                returnDataQurey = returnDataQurey.Where(r => r.FoodSource.Name == material);
            }

            if (price > 0)
            {
                returnDataQurey = returnDataQurey.Where(r => (r.ProductWeightInKilo * r.FoodSource.SourcePricePerKilo) <= price);
            }

            return View("Index", returnDataQurey.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Error");
            }

            Product product = db.Products.Where(r => r.Id == id.Value).Include(r => r.Stores).Include(r => r.FoodSource).FirstOrDefault();
            ViewBag.ProductStoreList = product.Stores.ToList();

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            var currentUser = (User)HttpContext.Session["user"];
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Error");
            }

            if (!currentUser.IsAdmin)
            {
                return RedirectToAction("Index", "Error", new { message = "Missing permissions" });
            }

            ViewBag.StoresList = new MultiSelectList(db.Stores.Select(i => new { Id = i.Id, Store = i.City + ", " + i.StreetAddress }), "Id", "Store");
            ViewBag.FoodSourceList = new SelectList(db.FoodSources.Select(i => new { Id = i.Id, Name = i.Name }), "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Product, FoodSourceId, StoresIds")] CreateProductsViewModel viewModel)
        {
            var foodSource = db.FoodSources.Find(viewModel.FoodSourceId);
            viewModel.Product.FoodSource = foodSource;

            // So the validation won't fail as it was null before we set it.
            ModelState.Remove("Product.FoodSource");

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Error", new { message = "Data wasn't valid, please try again" });
            }

            // Add the selected Stores
            viewModel.Product.Stores = new List<Store>();
            foreach (var storeId in viewModel.StoresIds)
            {
                var storeToAdd = db.Stores.Find(storeId);
                viewModel.Product.Stores.Add(storeToAdd);
            }

            db.Products.Add(viewModel.Product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            var curUser = (User)HttpContext.Session["user"];
            if (curUser == null)
            {
                return RedirectToAction("Index", "Error");
            }

            if (!curUser.IsAdmin)
            {
                return RedirectToAction("Index", "Error", new { message = "Missing permissions" });
            }

            if (id == null)
            {
                return RedirectToAction("Index", "Error");
            }

            Product product = db.Products.Where(r => r.Id == id).Include(r => r.FoodSource).Include(r => r.Stores).FirstOrDefault();
            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.storesList = new MultiSelectList(db.Stores.Select(i => new { Id = i.Id, Store = i.City + ", " + i.StreetAddress }), "Id", "Store");
            ViewBag.foodSourceList = new SelectList(db.FoodSources.Select(i => new { Id = i.Id, Name = i.Name }), "Id", "Name");
            CreateProductsViewModel viewModel = new CreateProductsViewModel
            {
                Product = product
            };

            viewModel.StoresIds.AddRange(from store in product.Stores
                                         select store.Id);
            viewModel.FoodSourceId = product.FoodSource.Id;

            return View(viewModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Product, FoodSourceId, StoresIds")] CreateProductsViewModel viewModel)
        {
            var curUser = (User)HttpContext.Session["user"];
            if (curUser != null)
            {
                return RedirectToAction("Index", "Error");
            }
            
            if (!curUser.IsAdmin)
            {
                return RedirectToAction("Index", "Error", new { message = "Missing Permissions" });
            }

            // So the validation wont fail as it was null before we set it.
            ModelState.Remove("Fabric.Material");

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            Product product = db.Products.Where(r => r.Id == viewModel.Product.Id).Include(r => r.FoodSource).Include(r => r.Stores).FirstOrDefault();

            product.Name = viewModel.Product.Name;
            product.ProductWeightInKilo = viewModel.Product.ProductWeightInKilo;
            var foodSource = db.FoodSources.Find(viewModel.FoodSourceId);
            product.FoodSource = foodSource;

            product.Stores = new List<Store>();
            foreach (var storeToAdd in from storeId in viewModel.StoresIds
                                        let storeToAdd = db.Stores.Find(storeId)
                                        select storeToAdd)
            {
                product.Stores.Add(storeToAdd);
            }

            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult ChangeFavorite(int id)
        {
            var currentUser = (User)HttpContext.Session["user"];
            if (currentUser == null)
            {
                throw new Exception("You are not logged in");
            }

            int prevFavorite = -1;
            User user = db.Users.Where(r => r.Id == currentUser.Id).Include(r => r.FavoriteProduct).FirstOrDefault(); // this way it will update the DB when done
            Product product = db.Products.Find(id);
            if (product == null)
            {
                throw new Exception($"Product id {id} was not found");
            }

            if (user.FavoriteProduct == null)
            {
                user.FavoriteProduct = product;
            }
            else
            {
                prevFavorite = user.FavoriteProduct.Id;
                // Removing or replacing favorite
                user.FavoriteProduct = prevFavorite == id ? null : product;
            }
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { PrevFavorite = "#" + prevFavorite });
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Error");
            }

            Product product = db.Products.Include(r => r.FoodSource).Where(r => r.Id == id).FirstOrDefault();
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product Product = db.Products.Find(id);
            var favoritedByUsersList = db.Users.Where(f => f.FavoriteProduct.Id == id).ToList();
            favoritedByUsersList.ForEach(x => x.FavoriteProduct = null);
            var purchasedList = db.Purchases.Where(f => f.Product.Id == id).ToList();
            purchasedList.ForEach(x => db.Purchases.Remove(x));
            db.Products.Remove(Product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

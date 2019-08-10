using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using anipet.Models;

namespace anipet.Controllers
{
    public class StoresController : Controller
    {
        private DBContext db = new DBContext();

        public class CreateStoreViewModel
        {
            public CreateStoreViewModel()
            {
                // Init the array incase there are no stores selected
                ProductIds = new List<int>();
            }
            public Store Store { get; set; }

            public List<int> ProductIds { get; set; }
        }

        // GET: Stores
        public ActionResult Index()
        {
            return View(db.Stores.ToList());
        }

        // GET: Stores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores.Where(r => r.Id == id.Value).Include(r => r.StoreAdmin).Include(r => r.Products).FirstOrDefault();

            ViewBag.storeProductList = store.Products.ToList();
            ViewBag.storeAdmin= store.StoreAdmin.Username;
            ViewBag.storeAdminId = store.StoreAdmin.Id;
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }


        // GET: Ingredients/Create
        public ActionResult Create()
        {
            var curUser = ((User)HttpContext.Session["user"]);
            if (curUser != null)
            {
                if (curUser.IsAdmin)
                {
                    ViewBag.ProductList = new MultiSelectList(db.Products.Select(i => new { Id = i.Id, Name = i.Name }), "Id", "Name");
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Error", new { message = "Missing permissions" });
                }
            }
            return RedirectToAction("Index", "Error");
        }

        // POST: Stores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Store, ProductIds")] CreateStoreViewModel viewModel)
        {
            var currentUser = ((User)HttpContext.Session["user"]);
            if (currentUser != null)
            {
                // Set the current user as the owner of the Store
                viewModel.Store.StoreAdmin = db.Users.Where(r => r.Id == currentUser.Id).Include(r => r.FavoriteProduct.FoodSource).FirstOrDefault();
                // So the validation wont fail as it was null before we set it.
                ModelState.Remove("Store.StoreAdmin");

                if (ModelState.IsValid)
                {
                    viewModel.Store.Products = new List<Product>();
                    foreach (var ProductId in viewModel.ProductIds)
                    {
                        var productToAdd = db.Products.Where(r => r.Id == ProductId).Include(r => r.FoodSource).FirstOrDefault();
                        viewModel.Store.Products.Add(productToAdd);
                    }

                    db.Stores.Add(viewModel.Store);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index", "Error");
            }

            return RedirectToAction("Index", "Error");
        }


        // GET: Stores/Edit/5
        public ActionResult Edit(int? id)
        {
            var curUser = ((User)HttpContext.Session["user"]);
            if (curUser != null)
            {
                if (curUser.IsAdmin)
                {
                    if (id == null)
                    {
                        return RedirectToAction("Index", "Error");
                    }
                    Store store = db.Stores.Where(r => r.Id == id).Include(r => r.Products).Include(r => r.StoreAdmin).FirstOrDefault();
                    if (store == null)
                    {
                        return HttpNotFound();
                    }

                    ViewBag.ProductList = new MultiSelectList(db.Products.Select(i => new { Id = i.Id, Name = i.Name }), "Id", "Name");

                    CreateStoreViewModel viewModel = new CreateStoreViewModel();
                    viewModel.Store = store;
                    foreach (var ring in store.Products)
                    {
                        viewModel.ProductIds.Add(ring.Id);
                    }

                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction("Index", "Error", new { message = "Missing permissions" });
                }
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Store, ProductIds")] CreateStoreViewModel viewModel)
        {
            var curUser = ((User)HttpContext.Session["user"]);
            if (curUser != null)
            {
                if (curUser.IsAdmin)
                {

                    // So the validation wont fail as it was null before we set it.
                    ModelState.Remove("Store.StoreAdmin");
                    ModelState.Remove("Store.StoreAdmin.Password");
                    ModelState.Remove("Store.StoreAdmin.Username");

                    if (ModelState.IsValid)
                    {
                        Store store = db.Stores.Where(r => r.Id == viewModel.Store.Id).Include(r => r.Products).Include(r => r.StoreAdmin).FirstOrDefault();

                        store.City = viewModel.Store.City;
                        store.StreetAddress = viewModel.Store.StreetAddress;
                        var StoreAdmin = db.Users.Where(r => r.Id == viewModel.Store.StoreAdmin.Id).Include(r => r.FavoriteProduct.FoodSource).FirstOrDefault();
                        store.StoreAdmin = StoreAdmin;

                        store.Products = new List<Product>();
                        foreach (var ProductId in viewModel.ProductIds)
                        {
                            var ringToAdd = db.Products.Find(ProductId);
                            store.Products.Add(ringToAdd);
                        }

                        db.Entry(store).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction("Index", "Error", new { message = "Missing permissions" });
                }
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Stores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        // POST: Stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Store store = db.Stores.Find(id);
            db.Stores.Remove(store);
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

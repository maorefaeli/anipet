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
    public class PurchasesController : Controller
    {
        private DBContext db = new DBContext();

        public class CreatePurchaseViewModel
        {
            public CreatePurchaseViewModel() { }
            public Purchase Purchase { get; set; }
            public int UserId { get; set; }
            public int[] ProductIds { get; set; }
        }

        // GET: Purchases
        public ActionResult Index()
        {
            var currentUser = ((User)HttpContext.Session["user"]);
            if (currentUser != null && currentUser.IsAdmin)
            {
                return View(db.Purchases.Include(r => r.Product).Include(r => r.User).OrderBy(r => r.PurchaseId).ToList());
            }
            else
            {
                return RedirectToAction("Index", "Error", new { message = "Missing permissions" });
            }
        }


        // GET: Purchases/Create
        public ActionResult Create()
        {
            var curUser = ((User)HttpContext.Session["user"]);
            if (curUser != null)
            {
                if (curUser.IsAdmin)
                {
                    ViewBag.ProductList = new MultiSelectList(db.Products.Select(i => new { Id = i.Id, Name = i.Name }), "Id", "Name");
                    ViewBag.UsersList = new MultiSelectList(db.Users.Select(i => new { Id = i.Id, Name = i.Username }), "Id", "Name");
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Error", new { message = "Missing permissions" });
                }
            }
            return RedirectToAction("Index", "Error");
        }

        // POST: Purchases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Purchase, UserId, ProductIds")] CreatePurchaseViewModel viewModel)
        {
            var user = db.Users.Find(viewModel.UserId);
            viewModel.Purchase.User = user;

            ModelState.Remove("Purchase.User");
            ModelState.Remove("Purchase.Product");

            if (ModelState.IsValid)
            {
                var date = DateTime.Now.Date;
                viewModel.Purchase.Date = date;

                foreach (var product_id in viewModel.ProductIds)
                {
                    var product = db.Products.Find(product_id);
                    viewModel.Purchase.Product = product;

                    db.Purchases.Add(viewModel.Purchase);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", "Error", new { message = "One or more inputs were invalid. Please try again." });
        }

        // GET: Purchases/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Error");
            }

            Purchase purchase = db.Purchases.Where(p => p.Id == id).Include(p => p.Product).FirstOrDefault();

            if (purchase == null)
            {
                return RedirectToAction("Index", "Error", new { message = "No such purchase exists." });
            }
            return View(purchase);
        }

        // POST: Purchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return RedirectToAction("Index", "Error", new { message = "No such purchase exists." });
            }

            db.Purchases.Remove(purchase);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

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
    public class FoodSourcesController : Controller
    {
        private DBContext db = new DBContext();

        // GET: FoodSources
        public ActionResult Index()
        {
            if ((User)HttpContext.Session["user"] != null)
            {
                return View(db.FoodSources.ToList());
            }

            return RedirectToAction("Index", "Error");
        }

        // GET: FoodSources/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Error");
            }

            FoodSource foodSource = db.FoodSources.Find(id);
            if (foodSource == null)
            {
                return HttpNotFound();
            }

            return View(foodSource);
        }

        // GET: FoodSources/Create
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

            return View();
        }

        // POST: FoodSources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id, Name, SourcePricePerKilo")] FoodSource foodSource)
        {
            if (ModelState.IsValid)
            {
                if (db.FoodSources.Where(c => c.Name.Equals(foodSource.Name)).Count() > 0)
                {
                    ViewBag.ErrMsg = "Food source already exists, please try again";
                }
                else if (foodSource.SourcePricePerKilo < 0)
                {
                    ViewBag.ErrMsg = "Price for food source isn't valid... please try again";
                }
                else
                {
                    db.FoodSources.Add(foodSource);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(foodSource);
        }

        // GET: FoodSources/Edit/5
        public ActionResult Edit(int? id)
        {
            var currentUser = ((User)HttpContext.Session["user"]);
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Error");
            }

            if (!currentUser.IsAdmin)
            {
                return RedirectToAction("Index", "Error", new { message = "Missing permissions" });
            }

            if (id == null)
            {
                return RedirectToAction("Index", "Error");
            }

            FoodSource foodSource = db.FoodSources.Find(id);
            if (foodSource == null)
            {
                return HttpNotFound();
            }
            return View(foodSource);
        }

        // POST: FoodSources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id, Name, SourcePricePerKilo")] FoodSource foodSource)
        {
            var currentUser = ((User)HttpContext.Session["user"]);
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Error");
            }

            if (!currentUser.IsAdmin)
            {
                return RedirectToAction("Index", "Error", new { message = "you are not authorized" });
            }

            if (!ModelState.IsValid)
            {
                return View(foodSource);
            }

            db.Entry(foodSource).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: FoodSources/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Error");
            }

            FoodSource foodSource = db.FoodSources.Find(id);
            if (foodSource == null)
            {
                return HttpNotFound();
            }
            return View(foodSource);
        }

        // POST: FoodSources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FoodSource foodSource = db.FoodSources.Find(id);
            try
            {
                db.FoodSources.Remove(foodSource);
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("Index", "Error", new { message = "Cannot delete food source being used by more then once" });
            }
            return RedirectToAction("Index");
        }

        // GET: FoodSources/Search
        public ActionResult Search(string foodSource = null, int price = 0)
        {
            var currentUser = (User)HttpContext.Session["user"];
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Error");
            }

            if (!currentUser.IsAdmin)
            {
                return RedirectToAction("Index", "Error", new { message = "Missing persmissions" });
            }

            var dataQurey = db.FoodSources.Include(r => r.ProductsFromFoodSource);

            if (!string.IsNullOrEmpty(foodSource))
            {
                dataQurey = dataQurey.Where(r => r.Name.Contains(foodSource));
            }

            if (price > 0)
            {
                dataQurey = dataQurey.Where(r => r.SourcePricePerKilo >= price);
            }

            return View("Index", dataQurey.ToList());
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

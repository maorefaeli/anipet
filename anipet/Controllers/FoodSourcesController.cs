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
            return View(db.FoodSources.ToList());
        }

        // GET: FoodSources/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
            return View();
        }

        // POST: FoodSources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,SourcePricePerKilo")] FoodSource foodSource)
        {
            if (ModelState.IsValid)
            {
                db.FoodSources.Add(foodSource);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(foodSource);
        }

        // GET: FoodSources/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
        public ActionResult Edit([Bind(Include = "Id,Name,SourcePricePerKilo")] FoodSource foodSource)
        {
            if (ModelState.IsValid)
            {
                db.Entry(foodSource).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(foodSource);
        }

        // GET: FoodSources/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
            db.FoodSources.Remove(foodSource);
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

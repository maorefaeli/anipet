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
    public class UsersController : Controller
    {
        private DBContext db = new DBContext();

        // GET: Users
        public ActionResult Index()
        {
            var currentUser = ((User)HttpContext.Session["user"]);
            if (currentUser != null)
            {
                var users = db.Users.Select(s => s);
                if (!currentUser.IsAdmin)
                {
                    users = users.Where(u => u.Id == currentUser.Id);
                }
                return View(users.Include(r => r.Stores).ToList());
            }
            return RedirectToAction("Index", "Error");
        }

        // GET: Users/Search
        public ActionResult Search(string username = null, int minStores = 0, string IsAdmin = "dont filter")
        {
            var currentUser = ((User)HttpContext.Session["user"]);
            if (currentUser != null)
            {
                if (currentUser.IsAdmin)
                {
                    var returnDataQurey = db.Users.Include(r => r.Stores);

                    if (!string.IsNullOrEmpty(username))
                    {
                        returnDataQurey = returnDataQurey.Where(r => r.Username.Contains(username));
                    }

                    if (IsAdmin != "dont filter")
                    {
                        bool admin = IsAdmin.Equals("True");
                        returnDataQurey = returnDataQurey.Where(r => r.IsAdmin == admin);
                    }

                    if (minStores > 0)
                    {
                        returnDataQurey = returnDataQurey.Where(r => r.Stores.Count() >= minStores);
                    }

                    return View("Index", returnDataQurey.ToList());
                }
                else
                {
                    return RedirectToAction("Index", "Error", new { message = "Missing permissions" });
                }
            }
            return RedirectToAction("Index", "Error");
        }


        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            ViewBag.userStoreList = db.Stores.Where(r => r.StoreAdmin.Id == id).ToList();
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            var currentUser = ((User)HttpContext.Session["user"]);
            if (currentUser == null)
            {
                return View();
            }
            else
            {
                if (currentUser.IsAdmin)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Error", new { message = "you are already logged in" });
                }
            }
        }

        // GET: Users/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string userName, string password)
        {
            var userInDataBase = db.Users.Include(r => r.FavoriteProduct).Where(s =>
                        s.Username.Equals(userName, System.StringComparison.Ordinal) &&
                        s.Password.Equals(password, System.StringComparison.Ordinal)).SingleOrDefault();
            if (userInDataBase != null)
            {
                System.Web.HttpContext.Current.Session["user"] = userInDataBase;
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrMsg = "User name or password are incorrect.";
            return View();
        }



        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Password,IsAdmin,Username")] User user)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Where(c => c.Username.Equals(user.Username)).Count() > 0)
                {
                    ViewBag.ErrMsg = "User name already exists, please try again";
                }
                else
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    if (System.Web.HttpContext.Current.Session["user"] == null)
                    {
                        System.Web.HttpContext.Current.Session["user"] = user;
                        return RedirectToAction("Index", "Stores");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            if (((User)System.Web.HttpContext.Current.Session["user"]).Id != id && ((User)System.Web.HttpContext.Current.Session["user"]).IsAdmin == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Password,IsAdmin,Username")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var userInDataBase = db.Users.Where(s =>
                       s.Id == id).SingleOrDefault();

            User user = db.Users.Find(id);
            try
            {
                db.Users.Remove(user);
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("Index", "Error", new { message = "ERROR - You are tring to delete user who owns one or more stores" });
            }

            if (userInDataBase != null)
            {
                if (((User)(System.Web.HttpContext.Current.Session["user"])).IsAdmin)
                    return RedirectToAction("Index");
                else
                {
                    System.Web.HttpContext.Current.Session["user"] = null;
                    return RedirectToAction("Login");
                }
            }
            ViewBag.ErrMsg = "User does not exist.";
            return View();
        }

        public ActionResult Logoff()
        {
            System.Web.HttpContext.Current.Session["user"] = null;
            return RedirectToAction("Login");
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

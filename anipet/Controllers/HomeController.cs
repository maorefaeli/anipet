using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace anipet.Controllers
{
    public class HomeController : Controller
    {
        private DBContext db = new DBContext();
        public ActionResult Index()
        {
            ViewBag.mostPopProduct = db.Users.GroupBy(r => r.FavoriteProduct).Select(g => new { Product = ((Product)g.Key), count = g.Count() }).OrderByDescending(r => r.count).First().Product;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public string GetAboutData()
        {
            var text = System.IO.File.ReadAllText(Server.MapPath("\\Content\\About\\aboutData.txt"));
            return text;
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Purchases()
        {
            return View();
        }
    }

}

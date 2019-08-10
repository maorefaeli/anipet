using anipet.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;


namespace anipet.Controllers
{
    public class StatsController : Controller
    {
        private DBContext db = new DBContext();

        [ChildActionOnly]
        public PartialViewResult getStats()
        {
            var ProductByFoodSource = db.Products.GroupBy(r => r.FoodSource).Select(g => new { foodSource = ((FoodSource)g.Key).Name, count = g.Count() }).ToList();
            var ProductByFoodSourceJSON = JsonConvert.SerializeObject(ProductByFoodSource);
            ViewBag.ProductByFoodSource = ProductByFoodSourceJSON;

            var ProductByStore = db.Stores.Select(r => new { store = r.City + "," + r.StreetAddress, count = r.Products.Count() }).ToList();
            var ProductByStoreJSON = JsonConvert.SerializeObject(ProductByStore);
            ViewBag.ProductByStore = ProductByStoreJSON;

            var StoresByCity = db.Stores.GroupBy(r => r.City).Select(g => new { city = g.Key.ToString(), count = g.Count() }).ToList();
            var StoresByCityJSON = JsonConvert.SerializeObject(ProductByFoodSource);
            ViewBag.StoresByCityJSON = StoresByCityJSON;

            return PartialView("~/Views/Stats/Stats.cshtml");
        }
    }
}
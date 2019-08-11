using anipet.Models;
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

        public class UserAndFood
        {
            public string UserName;
            public string FoodSourceName;
        }

        public class HomeIndexViewModel
        {
            public List<UserAndFood> UsersAndFood;
        }

        public ActionResult Index()
        {
            var popular = db.Users.GroupBy(r => r.FavoriteProduct)
                                  .Select(g => new { Product = g.Key, count = g.Count() })
                                  .OrderByDescending(r => r.count)
                                  .First().Product;
            ViewBag.MostPopularProduct = popular;

            var favoriteFood = db.Products
                .Join(db.Users,
                      product => product,
                      user => user.FavoriteProduct,
                      (product, user) => new {product, user})
                .Where(product => product.product.Id == popular.Id)
                .Select(z => new
                {
                    Name = z.user.Username,
                    FoodSource = z.product.FoodSource
                }).ToList();

            var vm = new HomeIndexViewModel
            {
                UsersAndFood = new List<UserAndFood>()
            };
            foreach (var item in favoriteFood)
            {
                var newItem = new UserAndFood
                {
                    UserName = item.Name,
                    FoodSourceName = item.FoodSource.Name
                };
                vm.UsersAndFood.Add(newItem);
            }

            return View(vm);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public string GetAboutData()
        {
            var text = System.IO.File.ReadAllText(Server.MapPath(@"\Content\About\data.txt"));
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

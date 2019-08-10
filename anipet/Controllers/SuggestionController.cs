using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using anipet.Models;
using DataAccess;
using libsvm;


namespace anipet.Controllers
{
    public class SuggestionController : Controller
    {
        private DBContext db = new DBContext();

        [ChildActionOnly]
        public PartialViewResult getSuggestion(string userName)
        {
            List<string> x = db.Purchases.OrderBy(p => p.Id).Select(p => p.User.Username).ToList();
            double[] y = db.Purchases.OrderBy(p => p.Id).Select(p => (double)p.Product.Id).ToArray();

            var users = db.Users.Select(s => s.Username).ToList();
            var problemBuilder = new TextClassificationProblemBuilder();
            var problem = problemBuilder.CreateProblem(x, y, users.ToList());

            const int C = 1;
            C_SVC model = new C_SVC(problem, KernelHelper.LinearKernel(), C);

            var newX = TextClassificationProblemBuilder.CreateNode(userName, users);

            var predictedY = model.Predict(newX);
            var prediction = db.Products.Find((int)predictedY);
            ViewBag.suggestion = prediction;

            return PartialView("~/Views/Suggestion/Suggestion.cshtml");
        }

        private static IEnumerable<string> GetWords(string x)
        {
            return x.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
using System.Web.Mvc;

namespace anipet.Controllers
{
    public class ErrorController : Controller
    {
        private const string DefaultErrorMessage = "You need to log in";

        // GET: Error
        public ActionResult Index(string message = null)
        {
            ViewBag.ErrorMessage = message ?? DefaultErrorMessage;
            return View();
        }
    }
}

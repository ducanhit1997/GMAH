using GMAH.Web.Helpers.Attributes;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Student.Controllers
{
    [RouteArea("Student", AreaPrefix = "")]
    public class HomeController : Controller
    {
        [JwtAuthentication]
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }
    }
}
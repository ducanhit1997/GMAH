using GMAH.Web.Helpers.Attributes;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Student.Controllers
{
    [RouteArea("Student", AreaPrefix = "")]
    [JwtAuthentication]
    public class ScoreController : Controller
    {
        [Route("diemhocsinh")]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [Route("diemcanhan")]
        [HttpGet]
        public ActionResult Self()
        {
            return View();
        }
    }
}
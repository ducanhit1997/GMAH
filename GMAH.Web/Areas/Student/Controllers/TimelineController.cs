using GMAH.Web.Helpers.Attributes;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Student.Controllers
{
    [RouteArea("Student", AreaPrefix = "")]
    [JwtAuthentication]
    public class TimelineController : Controller
    {
        [Route("thoikhoabieuhocsinh")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("thoikhoabieu")]
        public ActionResult Self()
        {
            return View();
        }

    }
}
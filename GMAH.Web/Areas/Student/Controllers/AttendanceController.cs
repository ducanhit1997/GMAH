using GMAH.Services.Services;
using GMAH.Web.Helpers.Attributes;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Student.Controllers
{
    [RouteArea("Student", AreaPrefix = "")]
    [JwtAuthentication]
    public class AttendanceController : Controller
    {
        [Route("xemdiemdanhhocsinh")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("xemdiemdanh")]
        public ActionResult Self()
        {
            return View();
        }
    }
}
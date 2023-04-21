using GMAH.Web.Helpers.Attributes;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Student.Controllers
{
    [RouteArea("Student", AreaPrefix = "")]
    [JwtAuthentication]
    public class InfoController : Controller
    {
        [Route("hocsinh")]
        [HttpGet]
        public ActionResult Student()
        {
            return View();
        }
    }
}
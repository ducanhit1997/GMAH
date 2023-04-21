using GMAH.Models.Consts;
using GMAH.Web.Helpers.Attributes;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "quantri")]
    public class HomeController : Controller
    {
        [Route("")]
        [Route("home")]
        [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER)]
        public ActionResult Index()
        {
            return View();
        }

        [Route("hometest")]
        [JwtAuthentication("hometest")]
        public ActionResult HomeTest()
        {
            return View("Index");
        }
    }
}
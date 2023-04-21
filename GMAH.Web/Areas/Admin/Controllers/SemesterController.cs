using GMAH.Models.Consts;
using GMAH.Web.Helpers.Attributes;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "quantri")]
    [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
    public class SemesterController : Controller
    {
        [Route("hocky")]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}
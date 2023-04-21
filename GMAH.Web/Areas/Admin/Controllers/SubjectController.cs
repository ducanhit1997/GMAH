using GMAH.Models.Consts;
using GMAH.Web.Helpers.Attributes;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "quantri")]
    public class SubjectController : Controller
    {
        [HttpGet]
        [Route("monhoc")]
        [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("giaovienbomon")]
        [Route("giaovienbomon/{id}")]
        [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT)]
        public ActionResult Teacher(int? id)
        {
            ViewBag.IdSubject = id is null ? 0 : id.Value;
            return View();
        }
    }
}
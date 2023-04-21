using GMAH.Models.Consts;
using GMAH.Web.Helpers.Attributes;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "quantri")]
    public class ReportController : Controller
    {
        [Route("danhsachbaocao")]
        [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public ActionResult Index()
        {
            return View();
        }

        [Route("duyetbaocao")]
        [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.TEACHER, RoleEnum.HEAD_OF_SUBJECT)]
        public ActionResult Review()
        {
            return View();
        }

        [Route("xembaocao")]
        [Route("xembaocao/{id}")]
        public ActionResult Info(int? id)
        {
            ViewBag.IdReport = id;
            return View();
        }
    }
}
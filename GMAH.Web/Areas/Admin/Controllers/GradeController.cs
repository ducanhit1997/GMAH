using GMAH.Models.Consts;
using GMAH.Services.Services;
using GMAH.Web.Helpers.Attributes;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "quantri")]
    [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
    public class GradeController : Controller
    {
        private SemesterService semesterService;

        public GradeController()
        {
            semesterService = new SemesterService();
        }

        [Route("luatxephang")]
        [Route("luatxephang/{id}")]
        [HttpGet]
        public ActionResult Index(int? id)
        {
            ViewBag.IdSemester = id ?? (semesterService.GetCurrentYearId() ?? 0);
            return View();
        }

        [Route("thongtinluat")]
        [Route("thongtinluat/{id}")]
        [HttpGet]
        public ActionResult Rule(int? id)
        {
            ViewBag.IdRule = id ?? 0;
            return View();
        }
    }
}
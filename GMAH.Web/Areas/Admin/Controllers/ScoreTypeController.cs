using GMAH.Models.Consts;
using GMAH.Services.Services;
using GMAH.Web.Helpers.Attributes;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "quantri")]
    public class ScoreTypeController : Controller
    {
        private SemesterService semesterService;

        public ScoreTypeController()
        {
            semesterService = new SemesterService();
        }

        [Route("thanhphandiem")]
        [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.TEACHER, RoleEnum.HEAD_OF_SUBJECT)]
        public ActionResult Index()
        {
            ViewBag.IdSemester = semesterService.GetCurrentYearId();
            return View();
        }
    }
}
using GMAH.Models.Consts;
using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using GMAH.Web.Helpers.Attributes;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "quantri")]
    public class TimelineController : Controller
    {
        private SemesterService semesterService;
        private ClassService classService;

        public TimelineController()
        {
            semesterService = new SemesterService();
            classService = new ClassService();
        }

        [Route("thoikhoabieu")]
        [Route("thoikhoabieu/{id}")]
        [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.TEACHER, RoleEnum.HEAD_OF_SUBJECT)]
        public ActionResult Index(int? id)
        {
            ViewBag.IdClass = id is null ? 0 : id.Value;
            ViewBag.IdSemester = semesterService.GetCurrentSemesterId() ?? null;

            if (id != null)
            {
                var classInfo = classService.GetClassById(id.Value);
                if (classInfo.IsSuccess)
                {
                    ViewBag.IdSemester = ((ClassViewModel)classInfo.Object).IdYear;
                }
            }

            return View();
        }
    }
}
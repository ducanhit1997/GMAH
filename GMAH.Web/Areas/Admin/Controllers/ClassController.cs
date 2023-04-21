using GMAH.Models.Consts;
using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using GMAH.Web.Helpers.Attributes;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "quantri")]
    public class ClassController : Controller
    {
        private SemesterService semesterService;
        private ClassService classService;

        public ClassController()
        {
            semesterService = new SemesterService();
            classService = new ClassService();
        }


        [Route("lop")]
        [Route("lop/{id}")]
        [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public ActionResult Index(int? id)
        {
            ViewBag.IdSemester = id ?? semesterService.GetCurrentYearId();
            return View();
        }

        [Route("thongtinlop")]
        [Route("thongtinlop/{id}")]
        [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public ActionResult Info(int? id)
        {
            ViewBag.IdClass = id ?? 0;
            ViewBag.IdSemester = semesterService.GetCurrentYearId();
            return View();
        }

        [Route("danhsachlop")]
        [Route("danhsachlop/{id}")]
        [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.TEACHER, RoleEnum.HEAD_OF_SUBJECT)]
        public ActionResult Student(int? id)
        {
            ViewBag.IdClass = id is null ? 0 : id.Value;
            ViewBag.IdSemester = semesterService.GetCurrentYearId() ?? null;

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
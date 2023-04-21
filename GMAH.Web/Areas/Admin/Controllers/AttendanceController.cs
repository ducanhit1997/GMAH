using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "quantri")]
    public class AttendanceController : Controller
    {
        private SemesterService semesterService;
        private ClassService classService;

        public AttendanceController()
        {
            semesterService = new SemesterService();
            classService = new ClassService();
        }

        [Route("diemdanh")]
        [Route("diemdanh/{id}")]
        public ActionResult Index(int? id)
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

        [Route("xemdiemdanh")]
        [Route("xemdiemdanh/{id}")]
        public ActionResult List(int? id)
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
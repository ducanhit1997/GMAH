using GMAH.Models.Consts;
using GMAH.Models.Models;
using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using GMAH.Web.Helpers;
using GMAH.Web.Helpers.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace GMAH.Web.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "quantri")]
    public class ScoreController : Controller
    {
        private SemesterService semesterService;
        private ScoreService scoreService;

        public ScoreController()
        {
            semesterService = new SemesterService();
            scoreService = new ScoreService();
        }

        [Route("xemdiem")]
        [Route("xemdiem/{id}")]
        [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.TEACHER, RoleEnum.HEAD_OF_SUBJECT)]
        public ActionResult Index(int? id)
        {
            ViewBag.IdClass = id is null ? 0 : id.Value;

            if (id == null)
            {
                ViewBag.IdSemester = semesterService.GetCurrentSemesterId();
                ViewBag.IdYear = semesterService.GetCurrentYearId();
            }

            return View();
        }

        [HttpGet]
        [Route("xuatdiem/{id}")]
        [JwtAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.TEACHER, RoleEnum.HEAD_OF_SUBJECT)]
        public ActionResult ExportScore(int id, int idSemester, ScoreViewTypeEnum viewType = ScoreViewTypeEnum.DETAIL)
        {
            var user = SessionHelper.GetSession<UserIdentity>("USER");
            var resultService = scoreService.GetClassScoreBySemester(user.IdUser, id, idSemester, viewType);
            if (!resultService.IsSuccess)
            {
                return HttpNotFound();
            }

            var data = resultService.Object as List<ScoreViewModel>;
            var buffer = ExcelHelper.ExportScoreToExcel(data, resultService.ScoreComponent) as MemoryStream;
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", "attachment; filename=Score.xlsx");
            Response.BinaryWrite(buffer.ToArray());
            Response.Flush();
            Response.End();
            return View("Index");
        }
    }
}
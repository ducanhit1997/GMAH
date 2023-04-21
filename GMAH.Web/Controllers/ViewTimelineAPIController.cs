using GMAH.Models.Consts;
using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using GMAH.Web.Helpers.Attributes;
using System;
using System.Security.Claims;
using System.Web.Http;

namespace GMAH.Web.Controllers
{
    [ApiAuthentication]
    public class ViewTimelineAPIController : ApiController
    {
        private TimelineService timelineService;
        private ClassService classService;

        public ViewTimelineAPIController()
        {
            timelineService = new TimelineService();
            classService = new ClassService();
        }

        [HttpGet]
        [ApiAuthentication(RoleEnum.PARENT, RoleEnum.STUDENT)]
        public BaseResponse GetTimelineDateRangeViewModel(int idStudent, int idSemester)
        {
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);

            // Học sinh tự xem tkb bản thân mình
            if (idStudent < 0) idStudent = userId;

            var idClass = classService.GetStudentClassInSemester(idStudent, idSemester);
            if (idClass is null)
            {
                return new BaseResponse("Học sinh không có lớp học trong học kỳ này");
            }

            return timelineService.GetTimelineDateRangeViewModel(idSemester, idClass.Value);
        }

        /// <summary>
        /// Lấy danh sách học sinh của phụ huynh
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthentication(RoleEnum.PARENT, RoleEnum.STUDENT)]
        public BaseResponse GetTimeLine(int idStudent, int idSemester, DateTime from, DateTime to)
        {
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);

            // Học sinh tự xem tkb bản thân mình
            if (idStudent < 0) idStudent = userId;

            var idClass = classService.GetStudentClassInSemester(idStudent, idSemester);
            if (idClass is null)
            {
                return new BaseResponse("Học sinh không có lớp học trong học kỳ này");
            }
            return timelineService.GetTimeline(idSemester, idClass.Value, from, to);
        }
    }
}

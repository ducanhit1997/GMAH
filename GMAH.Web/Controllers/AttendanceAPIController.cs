using GMAH.Models.Consts;
using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using GMAH.Web.Helpers.Attributes;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace GMAH.Web.Controllers
{
    public class AttendanceAPIController : ApiController
    {
        private AttendanceService attendanceService;
        private ClassService classService;

        public AttendanceAPIController()
        {
            attendanceService = new AttendanceService();
            classService = new ClassService();
        }

        [HttpGet]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public BaseResponse GetClassAttendanceByDate(int idClass, DateTime date)
        {
            return attendanceService.GetClassAttendance(idClass, date);
        }

        [HttpPost]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public BaseResponse SaveClassAttendanceByDate(ClassAttendanceViewModel data)
        {
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);

            data.AssistantID = userId;
            return attendanceService.SaveClassAttendance(data);
        }

        [HttpGet]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.TEACHER, RoleEnum.HEAD_OF_SUBJECT)]
        public BaseResponse GetStudentAttendance(int idClass, DateTime from, DateTime to)
        {
            var idStudents = classService.GetStudentVMInClass(idClass).Select(x => x.IdUser).ToList();
            return attendanceService.GetStudentAttendance(idClass, idStudents, from, to);
        }
    }
}

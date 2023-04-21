using GMAH.Models.Consts;
using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using GMAH.Web.Helpers.Attributes;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Http;

namespace GMAH.Web.Controllers
{
    [ApiAuthentication]
    public class ViewAttendanceAPIController : ApiController
    {
        private AttendanceService attendanceService;
        private ClassService classService;

        public ViewAttendanceAPIController()
        {
            attendanceService = new AttendanceService();
            classService = new ClassService();
        }

        [HttpGet]
        [ApiAuthentication(RoleEnum.PARENT, RoleEnum.STUDENT)]
        public BaseResponse ViewAttendance(int idStudent, int idYear, DateTime from, DateTime to)
        {
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);

            // Học sinh tự xem tkb bản thân mình
            if (idStudent < 0) idStudent = userId;

            var idClass = classService.GetStudentClassInYear(idStudent, idYear);
            if (idClass is null)
            {
                return new BaseResponse("Học sinh không có lớp học trong học kỳ này");
            }

            return attendanceService.GetStudentAttendance(idClass.Value, new List<int> { idStudent }, from, to);
        }
    }
}

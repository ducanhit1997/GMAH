using GMAH.Models.Consts;
using GMAH.Models.Models;
using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using GMAH.Web.Helpers.Attributes;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Http;

namespace GMAH.Web.Controllers
{
    public class ClassAPIController : ApiController
    {
        private ClassService classService;
        private SemesterService semesterService;
        private ScoreTypeService scoreTypeService;

        public ClassAPIController()
        {
            classService = new ClassService();
            semesterService = new SemesterService();
            scoreTypeService = new ScoreTypeService();
        }

        /// <summary>
        /// Lấy danh sách lớp theo học kỳ
        /// </summary>
        /// <param name="idSemester"></param>
        [HttpPost]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public PaginationResponse GetClassBySemester(int idSemester, DatatableParam param)
        {
            return classService.PaginationClassBySemester(idSemester, param);
        }

        /// <summary>
        /// Lấy thông tin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public BaseResponse GetClass(int id)
        {
            return classService.GetClassById(id);
        }

        /// <summary>
        /// Lưu thông tin
        /// </summary>
        /// <param name="data"></param>
        [HttpPost]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public BaseResponse SaveClass(ClassViewModel data)
        {
            return classService.SaveClass(data);
        }

        /// <summary>
        /// Xoá
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public BaseResponse DeleteClass(int id)
        {
            return classService.DeleteClass(id);
        }

        /// <summary>
        /// Lấy danh sách lớp mà giáo viên đó quản lý
        /// </summary>
        /// <param name="idSemester"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.TEACHER, RoleEnum.HEAD_OF_SUBJECT)]
        public GetTeacherClassBySemesterResponse GetTeacherClassBySemester(int? idSemester = null, int? idYear = null, bool viewScore = false)
        {
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);

            // Nếu ko có semester thì nghỉ
            if (idSemester is null && idYear is null)
            {
                return new GetTeacherClassBySemesterResponse
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy học kỳ hiện tại",
                };
            }

            return classService.GetTeacherClassBySemester(userId, idSemester, idYear, viewScore);
        }

        /// <summary>
        /// Lấy danh sách học sinh trong lớp
        /// </summary>
        /// <param name="idSemester"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.TEACHER, RoleEnum.HEAD_OF_SUBJECT)]
        public PaginationResponse GetStudentInClass(int idClass, DatatableParam param)
        {
            // Kiểm tra quyền trước khi xử lý
            //var userClaims = RequestContext.Principal as ClaimsPrincipal;
            //var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);
            //if (!classService.IsUserHavePermissionInClass(userId, idClass))
            //{
            //    return new PaginationResponse();
            //}

            // Xử lý
            return classService.GetStudentInClass(idClass, param);
        }

        /// <summary>
        /// Thêm học sinh vào lớp
        /// </summary>
        /// <param name="idSemester"></param>
        /// <returns></returns>
        [HttpPut]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.TEACHER, RoleEnum.HEAD_OF_SUBJECT)]
        public BaseResponse AddStudentToClass(AddOrRemoveStudentToClassRequest data)
        {
            // Kiểm tra quyền trước khi xử lý
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);
            if (!classService.IsUserHavePermissionInClass(userId, data.IdClass))
            {
                return new BaseResponse("Tài khoản không có quyền với lớp này");
            }

            // Xử lý
            return classService.AddStudentToClass(data.IdClass, data.IdUser);
        }

        /// <summary>
        /// Xoá học sinh ra khỏi lớp
        /// </summary>
        /// <param name="idSemester"></param>
        /// <returns></returns>
        [HttpDelete]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.TEACHER, RoleEnum.HEAD_OF_SUBJECT)]
        public BaseResponse RemoveStudentToClass(AddOrRemoveStudentToClassRequest data)
        {
            // Kiểm tra quyền trước khi xử lý
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);
            if (!classService.IsUserHavePermissionInClass(userId, data.IdClass))
            {
                return new BaseResponse("Tài khoản không có quyền với lớp này");
            }

            // Xử lý
            return classService.RemoveStudentToClass(data.IdClass, data.IdUser);
        }

        /// <summary>
        /// Lấy danh sách học sinh
        /// Lấy toàn bộ
        /// </summary>
        /// <param name="idClass"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.TEACHER, RoleEnum.HEAD_OF_SUBJECT)]
        public BaseResponse GetStudentVMInClass(int idClass)
        {
            // Xử lý
            return new BaseResponse
            {
                IsSuccess = true,
                Object = classService.GetStudentVMInClass(idClass),
            };
        }

        /// <summary>
        /// Lấy môn học mà giáo viên đang dạy trong lớp
        /// </summary>
        /// <param name="idClass"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.TEACHER, RoleEnum.HEAD_OF_SUBJECT)]
        public BaseResponse GetSubjectTeacherClass(int idClass)
        {
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);

            // Xử lý
            return new BaseResponse
            {
                IsSuccess = true,
                Object = classService.GetSubjectTeacherClass(idClass, userId),
            };
        }

        /// <summary>
        /// Lấy danh sách thành phần điểm
        /// </summary>
        [HttpPost]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.TEACHER, RoleEnum.HEAD_OF_SUBJECT)]
        public PaginationResponse GetClassSubjectScoreType(int idClass, int idSubject, DatatableParam param)
        {
            return scoreTypeService.PaginationData(idClass, idSubject, param);
        }

    }
}

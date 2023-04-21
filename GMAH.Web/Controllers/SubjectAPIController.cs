using GMAH.Models.Consts;
using GMAH.Models.Models;
using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using GMAH.Web.Helpers.Attributes;
using System.Security.Claims;
using System.Web.Http;

namespace GMAH.Web.Controllers
{
    public class SubjectAPIController : ApiController
    {
        private SubjectService subjectService;

        public SubjectAPIController()
        {
            subjectService = new SubjectService();
        }

        /// <summary>
        /// Lấy danh sách subject
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public PaginationResponse GetListSubject(DatatableParam param)
        {
            var result = subjectService.PaginationSubject(param);
            return result;
        }

        /// <summary>
        /// Lấy thông tin 1 subject bằng id
        /// </summary>
        /// <param name="id"></param>
        [HttpGet]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public BaseResponse GetSubject(int id)
        {
            return subjectService.GetSubjectById(id);
        }

        /// <summary>
        /// Lưu thông tin subject hoặc tạo mới
        /// </summary>
        /// <param name="data"></param>
        [HttpPost]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public BaseResponse SaveSubject(SubjectViewModel data)
        {
            return subjectService.SaveSubject(data);
        }

        /// <summary>
        /// Xoá subject
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public BaseResponse DeleteSubject(int id)
        {
            return subjectService.DeleteSubject(id);
        }

        /// <summary>
        /// Lấy thông tin giáo viên trong môn học, giáo viên bộ môn, trưởng bộ môn
        /// </summary>
        /// <param name="idSubject"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT)]
        public BaseResponse GetTeacherInSubject(int idSubject)
        {
            var result = subjectService.GetTeacherInSubject(idSubject);

            // Kiểm tra nếu role là head of subject thì ko được quyền chỉnh sửa list head of subject
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userRole = int.Parse(userClaims.FindFirst(x => x.Type == "IdRole").Value);
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);

            // Gán lại kết quả mới có set quyền hạn
            var teacherSubjectData = result.Object as TeacherInSubjectViewModel;
            teacherSubjectData.IsAllowEditHeadOfSubject = userRole != (int)RoleEnum.HEAD_OF_SUBJECT;

            // Lấy list subject mà user có quyền xem
            teacherSubjectData.Subjects = subjectService.GetListSubjectByUserId(userId);

            // Trả dữ liệu về front end
            result.Object = teacherSubjectData;
            return result;
        }

        /// <summary>
        /// Thêm giáo viên bộ môn
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT)]
        public BaseResponse SetTeacherSubject(SetTeacherSubjectRequest data)
        {
            return subjectService.SetTeacherInSubject(data.IdUser, data.IdSubject);
        }

        /// <summary>
        /// Thêm trưởng bộ môn
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public BaseResponse SetHeadOfSubject(SetTeacherSubjectRequest data)
        {
            return subjectService.SetHeadOfSubject(data.IdUser, data.IdSubject, data.FromYear, data.ToYear);
        }

        /// <summary>
        /// Xoá giáo viên bộ môn
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpDelete]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT)]
        public BaseResponse RemoveTeacherSubject(SetTeacherSubjectRequest data)
        {
            return subjectService.RemoveTecherInSubject(data.IdUser, data.IdSubject);
        }

        /// <summary>
        /// Xoá trưởng bộ môn
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpDelete]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public BaseResponse RemoveHeadOfSubject(SetTeacherSubjectRequest data)
        {
            return subjectService.RemoveHeadOfSubject(data.IdUser, data.IdSubject);
        }

        /// <summary>
        /// Hàm lấy toàn bộ danh sách môn học
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public BaseResponse GetAllSubject()
        {
            return subjectService.GetAllSubject();
        }

        /// <summary>
        /// Lấy danh sách môn học và teacher đảm nhiệm môn học
        /// </summary>
        [HttpGet]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public BaseResponse GetAllSubjectAndTeacher()
        {
            return subjectService.GetAllSubjectAndTeacher();
        }
    }
}

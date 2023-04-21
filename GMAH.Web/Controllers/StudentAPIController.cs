using GMAH.Models.Consts;
using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using GMAH.Web.Helpers.Attributes;
using System.Web.Http;

namespace GMAH.Web.Controllers
{
    public class StudentAPIController : ApiController
    {
        private ParentService parentService;

        public StudentAPIController()
        {
            parentService = new ParentService();
        }

        /// <summary>
        /// Lấy danh sách phụ huynh của học sinh
        /// </summary>
        /// <param name="idUser"></param>
        [HttpGet]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER, RoleEnum.STUDENT)]
        public BaseResponse GetParentByStudentID(int idUser)
        {
            return new BaseResponse
            {
                IsSuccess = true,
                Object = parentService.GetListParent(idUser),
            };
        }
    }
}

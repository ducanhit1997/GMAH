using GMAH.Models.Consts;
using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using GMAH.Web.Helpers.Attributes;
using System.Security.Claims;
using System.Web.Http;

namespace GMAH.Web.Controllers
{
    public class ParentAPIController : ApiController
    {
        private ParentService parentService;

        public ParentAPIController()
        {
            parentService = new ParentService();
        }

        /// <summary>
        /// Lấy danh sách học sinh của phụ huynh
        /// </summary>
        /// <param name="idUser"></param>
        [HttpGet]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER, RoleEnum.PARENT)]
        public BaseResponse GetChildByParentId(int idUser)
        {
            return new BaseResponse
            {
                IsSuccess = true,
                Object = parentService.GetListChild(idUser),
            };
        }

        /// <summary>
        /// Thêm học sinh cho phụ huynh
        /// </summary>
        /// <param name="idParent"></param>
        /// <param name="idChild"></param>
        [HttpPut]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public BaseResponse AddChildToParent(int idParent, int idChild)
        {
            return parentService.AddChildToParent(idParent, idChild);
        }

        /// <summary>
        /// Xoá học sinh khỏi parent
        /// </summary>
        /// <param name="idParent"></param>
        /// <param name="idChild"></param>
        [HttpDelete]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public BaseResponse RemoveChildFromParent(int idParent, int idChild)
        {
            return parentService.RemoveChildFromParent(idParent, idChild);
        }

        /// <summary>
        /// Thay đổi thông tin học sinh
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthentication(RoleEnum.PARENT)]
        public BaseResponse ChangeChildInfo(ChangeChildInfoRequest data)
        {
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            int idParent = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);
            data.IdParent = idParent;

            return parentService.ChangeChildInfo(data);
        }

        /// <summary>
        /// Xem chi tiết thông tin học sinh
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthentication(RoleEnum.PARENT)]
        public BaseResponse ViewChildInfo(int idChild)
        {
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            int idParent = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);

            return parentService.ViewChildInfo(idChild, idParent);
        }
    }
}

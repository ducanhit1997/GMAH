using GMAH.Models.Consts;
using GMAH.Models.Models;
using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using GMAH.Web.Helpers.Attributes;
using System.Security.Claims;
using System.Web.Http;

namespace GMAH.Web.Controllers
{
    [ApiAuthentication]
    public class ViewScoreAPIController : ApiController
    {
        private ParentService parentService;
        private ScoreService scoreService;

        public ViewScoreAPIController()
        {
            parentService = new ParentService();
            scoreService = new ScoreService();
        }

        /// <summary>
        /// Lấy danh sách học sinh của phụ huynh
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthentication(RoleEnum.PARENT)]
        public BaseResponse GetListChild()
        {
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            int idParent = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);

            return new BaseResponse
            {
                IsSuccess = true,
                Object = parentService.GetListChild(idParent),
            };
        }

        /// <summary>
        /// Lấy thông tin điểm của học sinh
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthentication(RoleEnum.PARENT, RoleEnum.STUDENT)]
        public GetClassScoreResponse GetScore(int idStudent, int idSemester, ScoreViewTypeEnum viewType = ScoreViewTypeEnum.DETAIL)
        {
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);

            // Học sinh tự xem điểm bản thân mình
            if (idStudent < 0) idStudent = userId;

            return scoreService.ParentGetChildScore(userId, idStudent, idSemester, viewType);
        }
    }
}

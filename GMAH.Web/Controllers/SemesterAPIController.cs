using GMAH.Models.Consts;
using GMAH.Models.Models;
using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using GMAH.Web.Helpers.Attributes;
using System.Web.Http;

namespace GMAH.Web.Controllers
{
    public class SemesterAPIController : ApiController
    {
        private SemesterService semesterService;

        public SemesterAPIController()
        {
            semesterService = new SemesterService();
        }

        /// <summary>
        /// Lấy danh sách semester
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public PaginationResponse GetListSemester(DatatableParam param)
        {
            var result = semesterService.PaginationSemester(param);
            return result;
        }

        /// <summary>
        /// Lấy thông tin 1 semester
        /// </summary>
        /// <param name="id"></param>
        [HttpGet]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public BaseResponse GetSemester(int id)
        {
            return semesterService.GetSemesterById(id);
        }

        /// <summary>
        /// Lưu thông tin học kỳ
        /// </summary>
        /// <param name="data"></param>
        [HttpPost]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public BaseResponse SaveSemester(SemesterViewModel data)
        {
            return semesterService.SaveSemester(data);
        }

        /// <summary>
        /// Xoá học kỳ
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public BaseResponse DeleteSemester(int id)
        {
            return semesterService.DeleteSemester(id);
        }

        /// <summary>
        /// Lưu current semester
        /// </summary>
        /// <param name="id"></param>
        [HttpPut]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
        public BaseResponse SetCurrentSemester(int id)
        {
            return semesterService.SetCurrentSemester(id);
        }

        [HttpGet]
        [ApiAuthentication]
        public SettingCurrentSemesterResponse GetCurrentSemester(ViewSemesterTypeEnum viewSemester = ViewSemesterTypeEnum.ONLY_SEMESTER)
        {
            return semesterService.GetAll(viewSemester);
        }

        [HttpGet]
        [ApiAuthentication]
        public BaseResponse GetSemesterInYear(int idSemester)
        {
            return new BaseResponse
            {
                IsSuccess = true,
                Object = semesterService.GetSemesterInYear(idSemester),
            };
        }
    }
}

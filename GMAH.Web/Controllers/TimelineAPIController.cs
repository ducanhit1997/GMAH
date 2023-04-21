using GMAH.Models.Consts;
using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using GMAH.Web.Helpers;
using GMAH.Web.Helpers.Attributes;
using System;
using System.IO;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace GMAH.Web.Controllers
{
    public class TimelineAPIController : ApiController
    {
        private TimelineService timelineService;

        public TimelineAPIController()
        {
            timelineService = new TimelineService();
        }

        [HttpGet]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.TEACHER, RoleEnum.HEAD_OF_SUBJECT)]
        public BaseResponse GetTimelineDateRangeViewModel(int idClass, int idSemester)
        {
            return timelineService.GetTimelineDateRangeViewModel(idSemester, idClass);
        }

        [HttpGet]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.TEACHER, RoleEnum.HEAD_OF_SUBJECT)]
        public BaseResponse GetTimeLine(int idClass, int idSemester, DateTime from, DateTime to)
        {
            return timelineService.GetTimeline(idSemester, idClass, from, to);
        }

        [HttpDelete]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.TEACHER, RoleEnum.HEAD_OF_SUBJECT)]
        public BaseResponse DeleteTimeline(int idClass, int idSemester)
        {
            return timelineService.ClearTimeline(idSemester, idClass);
        }

        [HttpPost]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER)]
        public BaseResponse ImportTimeline(int idClass, int idSemester)
        {
            // Kiểm tra file
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count != 1)
            {
                return new BaseResponse("Không tìm thấy file excel");
            }

            // Trích xuất file
            var excelFile = httpRequest.Files[0];
            var folderPath = HttpContext.Current.Server.MapPath("~/Assests/UploadTemp");
            Directory.CreateDirectory(folderPath);
            var filePath = HttpContext.Current.Server.MapPath("~/Assests/UploadTemp/temp_" + DateTime.Now.ToFileTimeUtc() + ".xlsx");
            excelFile.SaveAs(filePath);

            // Đọc file excel 
            var dataFromExcel = ExcelHelper.ReadTimelineFromExcel(filePath);
            if (!dataFromExcel.IsSuccess)
            {
                return new BaseResponse("Lỗi xử lý file excel. " + dataFromExcel.Message);
            }

            return timelineService.ImportTimeline(idClass, idSemester, dataFromExcel.TimelineData);
        }

    }
}

using GMAH.Models.Consts;
using GMAH.Models.Models;
using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using GMAH.Web.Helpers;
using GMAH.Web.Helpers.Attributes;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace GMAH.Web.Controllers
{
    [ApiAuthentication]
    public class ReportAPIController : ApiController
    {
        private ReportService reportService;
        private ClassService classService;
        private ScoreTypeService scoreTypeService;
        private SubjectService subjectService;

        public ReportAPIController()
        {
            reportService = new ReportService();
            classService = new ClassService();
            scoreTypeService = new ScoreTypeService();
            subjectService = new SubjectService();
        }

        [HttpPost]
        [ApiAuthentication(RoleEnum.PARENT, RoleEnum.STUDENT)]
        public BaseResponse SubmitReport(ReportViewModel data)
        {
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);

            data.IdUserSubmitReport = userId;

            // Upload evidence
            data.Files = new List<string>();
            var httpRequest = HttpContext.Current.Request;
            UploadFileHelper.ServerPath = HttpContext.Current.Server.MapPath("~");
            if (httpRequest.Files.Count > 0)
            {
                try
                {
                    foreach (string filename in httpRequest.Files)
                    {
                        var file = httpRequest.Files[filename];
                        var imgLink = UploadFileHelper.Upload(file);
                        data.Files.Add(imgLink);
                    }
                }
                catch (Exception ex)
                {
                    return new BaseResponse("Upload file bị lỗi. " + ex.Message);
                }
            }

            var result = reportService.SubmitReport(data);

            return result;
        }

        [HttpPost]
        [ApiAuthentication(RoleEnum.ASSISTANT, RoleEnum.MANAGER)]
        public PaginationResponse GetAllReport(int? status, DatatableParam param)
        {
            var result = reportService.GetListReport(null, status, param);
            return result;
        }

        [HttpPost]
        public PaginationResponse GetMyReport(int? status, DatatableParam param)
        {
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);

            var result = reportService.GetListReport(userId, status, param);
            return result;
        }

        [HttpPost]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.TEACHER, RoleEnum.HEAD_OF_SUBJECT)]
        public PaginationResponse GetReviewReport(DatatableParam param)
        {
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);

            var result = reportService.GetMyReviewReport(userId, param);
            return result;
        }

        [HttpGet]
        public BaseResponse GetScoreType(int idSemester, int idStudent, int idSubject)
        {
            var idClass = classService.GetStudentClassInSemester(idStudent, idSemester);
            if (idClass is null)
            {
                return new BaseResponse("Học sinh không có lớp trong học kỳ");
            }

            var data = scoreTypeService.PaginationData(idClass.Value, idSubject, new DatatableParam
            {
                length = 100,
                start = 0,
            });

            return new BaseResponse
            {
                IsSuccess = true,
                Object = data.data,
            };
        }

        [HttpGet]
        public BaseResponse GetSubjectInClass(int idStudent, int idSemester)
        {
            var idClass = classService.GetStudentClassInSemester(idStudent, idSemester);
            if (idClass is null)
            {
                return new BaseResponse("Học sinh không có lớp trong học kỳ");
            }

            return subjectService.GetClassSubject(idClass.Value);
        }

        [HttpPost]
        public BaseResponse SubmitReviewReport(ReviewViewModel data)
        {
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);
            data.IdUserUpdate = userId;

            return reportService.ReviewReport(data);
        }

        [HttpGet]
        public BaseResponse GetReport(int id)
        {
            // Kiểm tra role đủ quyền hạn không
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            int userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);

            return reportService.GetReport(id, userId);
        }
    }
}

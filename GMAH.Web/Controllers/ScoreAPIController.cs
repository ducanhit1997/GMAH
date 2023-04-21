using GMAH.Models.Consts;
using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using GMAH.Web.Helpers.Attributes;
using System.Net.Http;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using System.Web;
using System;
using GMAH.Web.Helpers;
using GMAH.Web.Helpers.Job;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Word;
using GMAH.Services.Utilities;
using System.Threading;

namespace GMAH.Web.Controllers
{
    public class ScoreAPIController : ApiController
    {
        private ScoreService scoreService;
        private BehaviourService behaviourService;
        private ScoreSemesterService scoreSemesterService;
        private ScoreTypeService scoreTypeService;
        private SystemSettingService settingService;
        private ClassService classService;

        public ScoreAPIController()
        {
            scoreService = new ScoreService();
            behaviourService = new BehaviourService();
            scoreSemesterService = new ScoreSemesterService();
            scoreTypeService = new ScoreTypeService();
            settingService = new SystemSettingService();
            classService = new ClassService();
        }

        [HttpGet]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER)]
        public GetClassScoreResponse GetClassScore(int idClass, int idSemester, ScoreViewTypeEnum viewType = ScoreViewTypeEnum.DETAIL)
        {
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);
            return scoreService.GetClassScoreBySemester(userId, idClass, idSemester, viewType);
        }

        [HttpGet]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER)]
        public StudentScoreResponse GetStudentScore(int idClass, int idStudent, int idSubject)
        {
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);
            var result = new StudentScoreResponse();
            var idSemester = (new SemesterService()).GetCurrentSemesterId();

            var allScore = scoreService.GetClassScoreBySemester(userId, idClass, idSemester ?? -1);
            if (!allScore.IsSuccess)
            {
                result.Message = allScore.Message;
                return result;
            }

            // Tìm điểm
            var scores = ((List<ScoreViewModel>)allScore.Object).Where(x => x.IdUser == idStudent).FirstOrDefault();
            if (scores is null)
            {
                result.Message = "Không tìm thấy điểm của học sinh này";
                return result;
            }

            var subjectScore = scores.Subjects.Where(x => x.IdSubject == idSubject).FirstOrDefault();
            if (scores is null)
            {
                result.Message = "Không tìm thấy điểm của học sinh này";
                return result;
            }

            result.ScoreComponents = new List<StudentScoreDetail>();
            foreach (var score in subjectScore.Details)
            {
                result.ScoreComponents.Add(new StudentScoreDetail
                {
                    Score = score.Score,
                    ScoreName = score.ScoreName,
                });
            }

            result.IsSuccess = true;
            return result;
        }

        [HttpPost]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER)]
        public BaseResponse SaveStudentScore(AddStudentScoreRequest data)
        {
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);

            var result = scoreService.AddStudentScore(userId, data.IdUser, data.IdSubject, data.IdSemester, data.ScoreTypeId, data.Score);

            // Send mail nếu score bị thay đổi trong học kỳ khác học kỳ hiện tại
            scoreService.CheckBaselineAndSendMail(data.IdUser, data.IdSemester);

            var listUpdated = new List<UpdatedScoreResponse>();
            listUpdated.Add(new UpdatedScoreResponse
            {
                IdUser = data.IdUser,
                IdScoreType = data.ScoreTypeId,
            });
            result.Object = listUpdated;
            return result;
        }

        /// <summary>
        /// Save điểm cho mobile
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER)]
        public BaseResponse MobileSaveStudentScore(List<AddStudentScoreRequest> listScore)
        {
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);

            foreach (var data in listScore)
            {
                var result = scoreService.AddStudentScore(userId, data.IdUser, data.IdSubject, data.IdSemester, data.ScoreTypeId, data.Score);
                if (!result.IsSuccess) return result;
            }

            return new BaseResponse
            {
                IsSuccess = true,
            };
        }

        /// <summary>
        /// API import score từ file excel
        /// https://stackoverflow.com/questions/33387764/how-to-post-file-to-asp-net-web-api-2
        /// </summary>
        [HttpPost]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER)]
        public BaseResponse CheckImportScore(int idClass, int idSemester)
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
            var dataFromExcel = ExcelHelper.ReadScoreFromExcel(filePath);
            if (!dataFromExcel.IsSuccess)
            {
                return new BaseResponse("Lỗi xử lý file excel. " + dataFromExcel.Message);
            }

            // Lưu điểm
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);
            return scoreService.CheckImportScore(userId, idClass, idSemester, dataFromExcel);
        }

        /// <summary>
        /// API import score từ file excel
        /// https://stackoverflow.com/questions/33387764/how-to-post-file-to-asp-net-web-api-2
        /// </summary>
        [HttpPost]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER)]
        public BaseResponse ImportScore(int idClass, int idSemester, bool confirm = false)
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
            var dataFromExcel = ExcelHelper.ReadScoreFromExcel(filePath);
            if (!dataFromExcel.IsSuccess)
            {
                return new BaseResponse("Lỗi xử lý file excel. " + dataFromExcel.Message);
            }

            // Lưu điểm
            var userClaims = RequestContext.Principal as ClaimsPrincipal;
            var userId = int.Parse(userClaims.FindFirst(x => x.Type == "IdUser").Value);

            return confirm ? scoreService.ImportScoreFromExcel(userId, idClass, idSemester, dataFromExcel) : scoreService.CheckImportScore(userId, idClass, idSemester, dataFromExcel);
        }

        public BaseResponse SaveStudentBehaviour(SaveStudentBehaviourRequest data)
        {
            var result = behaviourService.SetBehaviourRank(data.IdUser, data.IdSemester, data.IdYear, data.IdBehaviour);
            return result;
        }

        /// <summary>
        /// Method để phục vụ debug job
        /// Chỉ allow role manager
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthentication(RoleEnum.MANAGER)]
        public BaseResponse ExecuteUpdateScoreJob()
        {
            UpdateScoreJob job = new UpdateScoreJob();
            job.Execute(null).Wait();
            return new BaseResponse
            {
                IsSuccess = true,
                Message = "Executed",
            };
        }

        /// <summary>
        /// Lấy log sửa điểm
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="idSemester"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER)]
        public BaseResponse GetScoreLog(int idUser, int idSemester)
        {
            return scoreService.GetScoreLog(idUser, idSemester);
        }

        /// <summary>
        /// Gửi mail cho phụ huynh
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="idSemester"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER)]
        public BaseResponse SendMailScore(int? idUser, int? idSemester, int idYear, int idClass)
        {
            // Lấy score
            try
            {
                var listIdUser = classService.GetStudentVMInClass(idClass).Select(x => x.IdUser).ToList();
                if (idUser != null)
                {
                    listIdUser = new List<int> { idUser.Value };
                }

                var folderPath = HttpContext.Current.Server.MapPath("~/Assests/UploadTemp");
                var filePath = HttpContext.Current.Server.MapPath("~/Assests/Data/ScoreMailMerge.docx");

                Directory.CreateDirectory(folderPath);

                Thread trd = new Thread(new ThreadStart(() =>
                {
                    foreach (var idStudentUser in listIdUser)
                    {
                        var scoreDetail = scoreSemesterService.GetSemesterRank(idStudentUser, idSemester, idYear);
                        if (scoreDetail is null) return;

                        // Mail merge, nếu fail thì gửi mail bình thường
                        string fileScorePath = null;
                        string content = null;
                        try
                        {
                            // Copy template
                            fileScorePath = folderPath + "/Score_" + DateTime.Now.ToFileTimeUtc() + ".docx";
                            File.Copy(filePath, fileScorePath);

                            // Mail merge
                            var fields = new Dictionary<string, string>();
                            fields.Add("StudentName", scoreDetail.StudentName);
                            fields.Add("Semester", scoreDetail.SubjectName);
                            fields.Add("Year", scoreDetail.YearName);
                            fields.Add("Score", scoreDetail.Details.Where(x => x.ScoreName == "Score").First().Text);
                            fields.Add("Behaviour", scoreDetail.Details.Where(x => x.ScoreName == "Behaviour").First().Text);
                            fields.Add("Rank", scoreDetail.Details.Where(x => x.ScoreName == "Rank").First().Text);

                            MailMergeHelper.TextToWord(fileScorePath, fields);
                        }
                        catch
                        {
                            // Any exception
                            fileScorePath = null;

                            var emailTemplate = settingService.GetSetting(SettingEnum.EMAIL_TEMPLATE_SCORE) ?? String.Empty;
                            emailTemplate = emailTemplate.Replace("{fullname}", scoreDetail.StudentName);
                            emailTemplate = emailTemplate.Replace("{semester}", scoreDetail.SubjectName);
                            emailTemplate = emailTemplate.Replace("{score}", scoreDetail.Details.Where(x => x.ScoreName == "Score").First().Text);
                            emailTemplate = emailTemplate.Replace("{behaviour}", scoreDetail.Details.Where(x => x.ScoreName == "Behaviour").First().Text);
                            emailTemplate = emailTemplate.Replace("{rank}", scoreDetail.Details.Where(x => x.ScoreName == "Rank").First().Text);

                            content = emailTemplate;
                        }

                        // Send mail
                        scoreSemesterService.SendMailScore(fileScorePath?.Replace(".docx", ".pdf"), idStudentUser, content);
                    }
                }));
                trd.IsBackground = true;
                trd.Start();
            }
            catch
            {
                // Do nothing
            }

            return new BaseResponse
            {
                IsSuccess = true,
                Message = idUser is null ? "Gửi mail cho lớp hoàn tất, lưu ý: chỉ những học sinh có điểm và có phụ huynh hệ thống mới tiến hành gửi mail, có thể mất 1 thời gian để mail đến được" : "Gửi mail hoàn tất, có thể mất 1 thời gian để mail đến được",
            };
        }

        /// <summary>
        /// Lưu thông tin score type
        /// </summary>
        /// <param name="data"></param>
        [HttpPost]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER)]
        public BaseResponse SaveScoreType(ScoreTypeRequest data)
        {
            return scoreTypeService.SaveScoreDetail(data);
        }

        /// <summary>
        /// Lấy thông tin 1 subject score type
        /// </summary>
        /// <param name="id"></param>
        [HttpGet]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER)]
        public BaseResponse GetScoreType(int id)
        {
            return scoreTypeService.GetScoreType(id);
        }

        /// <summary>
        /// Xoá thông tin 1 subject score type
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete]
        [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT, RoleEnum.HEAD_OF_SUBJECT, RoleEnum.TEACHER)]
        public BaseResponse DeleteScoreType(int id)
        {
            return scoreTypeService.DeleteScoreType(id);
        }


    }
}

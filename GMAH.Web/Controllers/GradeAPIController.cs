using GMAH.Models.Consts;
using GMAH.Models.ViewModels;
using GMAH.Services.Services;
using GMAH.Services.Utilities;
using GMAH.Web.Helpers.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlTypes;
using System.Web.Http;

namespace GMAH.Web.Controllers
{
    [ApiAuthentication(RoleEnum.MANAGER, RoleEnum.ASSISTANT)]
    public class GradeAPIController : ApiController
    {
        private GradeService gradeService;
        private SemesterService semesterService;

        public GradeAPIController()
        {
            gradeService = new GradeService();
            semesterService = new SemesterService();
        }

        [HttpGet]
        public BaseResponse GetAll(int idSemester)
        {
            return gradeService.GetAllGradeRuleBySemester(idSemester);
        }

        [HttpGet]
        public GetGradeRuleResponse GetGradeRuleById(int idRule)
        {
            // Nếu ko có id rule thì trả về empty view model
            if (idRule == 0)
            {
                var emptyResponse = new GetGradeRuleResponse
                {
                    IdSemester = semesterService.GetCurrentYearId() ?? 0,
                };

                // Nạp dữ liệu từng loại xếp hạng
                var emptyVM = new List<GradeRuleViewModel>();
                foreach (RankEnum grade in Enum.GetValues(typeof(RankEnum)))
                {
                    emptyVM.Add(new GradeRuleViewModel
                    {
                        IdRank = grade,
                        IdSemester = emptyResponse.IdSemester,
                        GradeName = grade.GetAttribute<DisplayAttribute>().Name,
                    });
                }

                emptyResponse.Object = emptyVM;
                emptyResponse.IsSuccess = true;
                return emptyResponse;
            }
            else
            {
                return gradeService.GetGradeRuleById(idRule);
            }
        }

        [HttpPost]
        public BaseResponse Save(SaveGradeRuleRequest data)
        {
            if (data is null || data.Data is null)
            {
                return new BaseResponse("Dữ liệu không được bỏ trống");
            }
            return gradeService.SaveGradeRule(data.Data);
        }

        [HttpDelete]
        public BaseResponse Delete(int id)
        {
            return gradeService.DeleteRule(id);
        }
    }
}

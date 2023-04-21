using GMAH.Entities;
using GMAH.Models.Consts;
using GMAH.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;

namespace GMAH.Services.Services
{
    public class GradeService : BaseService
    {
        /// <summary>
        /// Lấy danh sách rule thông qua semester
        /// </summary>
        /// <param name="idSemester"></param>
        public BaseResponse GetAllGradeRuleBySemester(int idSemester)
        {
            // Kiểm tra semester
            if (!_db.SEMESTERs.Any(x => x.IdSemester == idSemester))
            {
                return new BaseResponse("Học kỳ này không tồn tại");
            }

            // Lấy grade rule
            var gradeRuleDB = _db.GRADERULEs.Where(x => x.IdSemester == idSemester).FirstOrDefault();

            // Nếu tồn tại thì trả về dữ liệu
            return new BaseResponse
            {
                IsSuccess = true,
                Object = gradeRuleDB is null ? null : ConvertToViewModel(gradeRuleDB).Distinct().ToList(),
            };
        }

        /// <summary>
        /// Lấy luật thông qua mã lớp mà học kỳ
        /// </summary>
        /// <param name="idSemester"></param>
        /// <param name="idClass"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public GetGradeRuleResponse GetGradeRuleById(int idRule)
        {
            // Kiểm tra dữ liệu
            var ruleDB = _db.GRADERULEs.Where(x => x.IdRule == idRule).FirstOrDefault();
            if (ruleDB is null)
            {
                return new GetGradeRuleResponse
                {
                    IsSuccess = false,
                    Message = "Dữ liệu này không tồn tại",
                };
            }

            // Nếu tồn tại thì trả về dữ liệu
            var resultVM = ruleDB is null ? null : ConvertToViewModel(ruleDB);
            return new GetGradeRuleResponse
            {
                IsSuccess = true,
                IdClass = resultVM.FirstOrDefault()?.IdClass ?? null,
                IdSemester = resultVM.FirstOrDefault()?.IdSemester ?? 0,
                Object = resultVM,
            };

        }

        /// <summary>
        /// Lưu lại luật xếp hạng
        /// </summary>
        /// <param name="data"></param>
        public BaseResponse SaveGradeRule(List<GradeRuleViewModel> data)
        {
            var idSemester = data.FirstOrDefault()?.IdSemester ?? -1;
            var idRule = data.FirstOrDefault()?.IdRule ?? -1;
            var idClass = data.FirstOrDefault()?.IdClass ?? new List<int>();

            // Kiểm tra data tồn tại không
            // Kiểm tra semester
            if (!_db.SEMESTERs.Any(x => x.IdSemester == idSemester))
            {
                return new BaseResponse("Học kỳ này không tồn tại");
            }

            // Kiểm tra rule nếu có
            var ruleDB = _db.GRADERULEs.Where(x => x.IdRule == idRule).FirstOrDefault();
            if (ruleDB is null)
            {
                // Trường hợp tạo mới
                if (idRule == 0)
                {
                    ruleDB = new GRADERULE();
                    _db.GRADERULEs.Add(ruleDB);
                }
                else
                {
                    return new BaseResponse("Luật xếp hạng này không tồn tại");
                }
            }

            // Kiểm tra dữ liệu class
            var checkClass = _db.GRADERULEs.Where(x => x.IdSemester == idSemester && x.IdRule != idRule).SelectMany(x => x.CLASSes).ToList();
            if (checkClass.Select(x => x.IdClass).Any(x => idClass.Any(i => i == x)))
            {
                return new BaseResponse("Không thể tạo luật này vì bị xung đột mã lớp");
            }

            // Gán lớp
            var addClass = _db.CLASSes.Where(x => idClass.Any(i => i == x.IdClass)).ToList();
            addClass.ForEach(x => ruleDB.CLASSes.Add(x));

            // Gán dữ liệu cho ruleDB
            ruleDB.IdSemester = idSemester;

            // Tạo các dữ liệu xếp hạng
            foreach (RankEnum grade in Enum.GetValues(typeof(RankEnum)))
            {
                var gradeRuleDB = ruleDB.GRADERULELISTs.Where(x => x.IdRank == (int)grade).FirstOrDefault();
                if (gradeRuleDB is null)
                {
                    gradeRuleDB = new GRADERULELIST
                    {
                        IdRank = (int)grade,
                    };
                    ruleDB.GRADERULELISTs.Add(gradeRuleDB);
                }

                // Nạp dữ liệu
                gradeRuleDB.MinAvgScore = data.Where(x => x.IdRank == grade).FirstOrDefault()?.MinAvgScore;
                gradeRuleDB.IdBehaviour = data.Where(x => x.IdRank == grade).FirstOrDefault()?.IdBehaviour;

                // Lưu một phần dữ liệu
                _db.GRADERULEDETAILs.RemoveRange(gradeRuleDB.GRADERULEDETAILs);
                _db.SaveChanges();

                // Nạp dữ liệu xếp hạng
                var gradeRuleDetailData = data.Where(x => x.IdRank == grade).FirstOrDefault();
                if (gradeRuleDetailData is null || gradeRuleDetailData.Details is null)
                {
                    continue;
                }

                foreach (var detail in gradeRuleDetailData.Details)
                {
                    gradeRuleDB.GRADERULEDETAILs.Add(new GRADERULEDETAIL
                    {
                        IdSubject = detail.IdSubject,
                        MinAvgScore = detail.MinAvgScore,
                    });
                }
            }

            // Lưu lại dữ liệu
            try
            {
                // Lưu lại
                _db.SaveChanges();

                // Thành công
                return new BaseResponse
                {
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                // Lưu db thất bại
                return new BaseResponse("Thao tác CSDL thất bại, mô tả lỗi từ hệ thống: " + ex.Message);
            }
        }

        /// <summary>
        /// Xoá rule bằng Id
        /// </summary>
        /// <param name="id"></param>
        public BaseResponse DeleteRule(int id)
        {
            // Kiểm tra dữ liệu có tồn tại không
            var ruleDB = _db.GRADERULEs.Where(x => x.IdRule == id).FirstOrDefault();
            if (ruleDB is null)
            {
                // Báo lỗi tìm ko thấy
                return new BaseResponse("Không tìm thấy dữ liệu này");
            }

            // Xoá rule khỏi các class
            foreach (var classDB in ruleDB.CLASSes)
            {
                classDB.IdRule = null;
            }

            // Xoá các dữ liệu liên quan semester
            var classesDB = _db.CLASSes.Where(x => x.IdRule == ruleDB.IdRule).ToList();
            foreach (var classDB in classesDB)
            {
                classDB.IdRule = null;
            }

            // Hard delete
            _db.GRADERULEs.Remove(ruleDB);

            // Lưu lại dữ liệu
            try
            {
                // Lưu lại
                _db.SaveChanges();

                // Thành công
                return new BaseResponse
                {
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                // Lưu db thất bại
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Thao tác CSDL thất bại, mô tả lỗi từ hệ thống: " + ex.Message,
                };
            }
        }
    }
}

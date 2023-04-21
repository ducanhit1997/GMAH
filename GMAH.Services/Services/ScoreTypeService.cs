using GMAH.Entities;
using GMAH.Models.Models;
using GMAH.Models.ViewModels;
using GMAH.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GMAH.Services.Services
{
    public class ScoreTypeService : BaseService
    {
        /// <summary>
        /// Lấy danh sách cột điểm
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public PaginationResponse PaginationData(int idClass, int idSubject, DatatableParam filter)
        {
            var listVM = new List<ScoreTypeViewModel>();
            var classSubjectDB = _db.CLASS_SUBJECT.Where(x => x.IdClass == idClass && x.IdSubject == idSubject).FirstOrDefault();
            if (classSubjectDB is null)
            {
                return new PaginationResponse
                {
                    draw = filter.draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = listVM,
                };
            }

            // Phân trang
            var listScoreType = classSubjectDB.SCORE_TYPE.Where(x => x.ScoreWeight != null).OrderBy(x => x.ScoreWeight).ThenByDescending(x => x.IdScoreType).ToList();

            // Search by value
            if (!string.IsNullOrEmpty(filter.search?.Value))
            {
                string value = filter.search?.Value;
                listScoreType = listScoreType.Where(x => x.ScoreName.Contains(value)).ToList();
            }

            var data = listScoreType.Skip(filter.start).Take(filter.length).ToList();

            // Convert danh sách
            foreach (var scoreType in data)
            {
                listVM.Add(ConvertToViewModel(scoreType));
            }

            return new PaginationResponse
            {
                draw = filter.draw,
                recordsTotal = listScoreType.Count(),
                recordsFiltered = listVM.Count,
                data = listVM,
            };
        }

        /// <summary>
        /// Tạo các cột điểm
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public BaseResponse SaveScoreDetail(ScoreTypeRequest data)
        {
            // Kiểm tra model
            var validateModel = ValidationModelUtility.Validate(data);
            if (!validateModel.IsValidate)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = validateModel.ErrorMessage,
                };
            }

            var scoreTypeDB = _db.SCORE_TYPE.Where(x => x.IdScoreType == data.IdScoreType).FirstOrDefault();

            var classSubjectDB = _db.CLASS_SUBJECT.Where(x => x.IdClass == data.IdClass && x.IdSubject == data.IdSubject).FirstOrDefault();
            if (classSubjectDB is null)
            {
                return new BaseResponse("Không tìm thấy dữ liệu lớp học hoặc môn học");
            }

            if (classSubjectDB.SCORE_TYPE.Any(x => x.IdScoreType != data.IdScoreType && x.ScoreName.Equals(data.ScoreName, StringComparison.OrdinalIgnoreCase)))
            {
                return new BaseResponse($"Đã tồn tại cột điểm {data.ScoreName} trước đó");
            }

            if (scoreTypeDB is null)
            {
                scoreTypeDB = new SCORE_TYPE
                {
                    IdClassSubject = classSubjectDB.IdClassSubject,
                    ScoreWeight = data.ScoreWeight,
                    ScoreName = data.ScoreName,
                };

                classSubjectDB.SCORE_TYPE.Add(scoreTypeDB);
            }

            scoreTypeDB.ScoreWeight = data.ScoreWeight;
            scoreTypeDB.ScoreName = data.ScoreName;

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
        /// Lấy các cột điểm
        /// </summary>
        /// <param name="idClassSubject"></param>
        /// <returns></returns>
        public List<SCORE_TYPE> GetScoreTypeByClassSubject(int idClassSubject)
        {
            var listScoreType = _db.SCORE_TYPE.Where(x => x.IdClassSubject == idClassSubject).OrderBy(x => x.ScoreWeight ?? 0).ToList();
            return listScoreType;
        }

        public BaseResponse GetScoreType(int id)
        {
            var scoreTypeDB = _db.SCORE_TYPE.AsNoTracking().Where(x => x.IdScoreType == id).FirstOrDefault();

            // Báo lỗi nếu user ko tồn tại
            if (scoreTypeDB is null)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Dữ liệu không tồn tại",
                };
            }

            // Trả về user
            return new BaseResponse
            {
                IsSuccess = true,
                Object = ConvertToViewModel(scoreTypeDB),
            };
        }

        /// <summary>
        /// Xoá score type bằng Id
        /// </summary>
        /// <param name="id"></param>
        public BaseResponse DeleteScoreType(int id)
        {
            // Kiểm tra dữ liệu có tồn tại không
            var scoreTypeDB = _db.SCORE_TYPE.Where(x => x.IdScoreType == id).FirstOrDefault();
            if (scoreTypeDB is null)
            {
                // Báo lỗi tìm ko thấy
                return new BaseResponse("Không tìm thấy dữ liệu này");
            }

            // Không thể xoá nếu có giáo viên bên đang phụ trách môn học
            if (scoreTypeDB.SCOREs.Any())
            {
                // Báo lỗi tìm ko thấy
                return new BaseResponse("Điểm đã được áp dụng, không thể xoá");
            }

            // Hard delete
            _db.SCORE_TYPE.Remove(scoreTypeDB);

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

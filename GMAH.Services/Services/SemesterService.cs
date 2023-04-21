using GMAH.Entities;
using GMAH.Models.Consts;
using GMAH.Models.Models;
using GMAH.Models.ViewModels;
using GMAH.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GMAH.Services.Services
{
    /// <summary>
    /// Xử lý liên quan đến dữ liệu học kỳ
    /// </summary>
    public class SemesterService : BaseService
    {
        /// <summary>
        /// Lấy dữ liệu semester, có phân trang
        /// </summary>
        /// <returns></returns>
        public PaginationResponse PaginationSemester(DatatableParam filter)
        {
            // Lấy danh sách semester
            // Sắp xếp theo ngày kết thúc
            var listSemester = _db.SEMESTERs.AsNoTracking().OrderByDescending(x => x.DateEnd).ToList();

            // Search by value
            if (!string.IsNullOrEmpty(filter.search?.Value))
            {
                string value = filter.search?.Value;
                listSemester = listSemester.Where(x => x.SemesterName.Contains(value)).ToList();
            }

            // Phân trang
            var data = listSemester.Skip(filter.start).Take(filter.length).ToList();

            // Convert danh sách theo role
            var listVM = new List<SemesterViewModel>();
            foreach (var semester in data)
            {
                listVM.Add(ConvertToViewModel(semester));
            }

            return new PaginationResponse
            {
                draw = filter.draw,
                recordsTotal = listSemester.Count(),
                recordsFiltered = listVM.Count,
                data = listVM,
            };
        }

        /// <summary>
        /// Lưu semester
        /// </summary>
        /// <param name="data"></param>
        public BaseResponse SaveSemester(SemesterViewModel data)
        {
            // Kiểm tra model
            var validateModel = ValidationModelUtility.Validate(data);
            if (!validateModel.IsValidate)
            {
                return new BaseResponse(validateModel.ErrorMessage);
            }

            // Kiểm tra dữ liệu có tồn tại không
            var semesterDB = _db.SEMESTERs.Where(x => x.IdSemester == data.IdSemester).FirstOrDefault();
            if (semesterDB is null)
            {
                // Báo lỗi tìm ko thấy
                if (data.IdSemester > 0)
                {
                    return new BaseResponse("Không tìm thấy dữ liệu này");
                }
                else
                {
                    // Tạo mới
                    semesterDB = new SEMESTER();
                    _db.SEMESTERs.Add(semesterDB);
                }
            }

            // Thay đổi dữ liệu
            semesterDB.SemesterName = data.SemesterName;
            semesterDB.ScoreWeight = data.ScoreWeight;
            semesterDB.DateEnd = data.DateEnd;
            semesterDB.DateStart = data.DateStart;
            semesterDB.IsCurrentSemester = data.IsCurrentSemester;

            if (data.IsCurrentSemester == true)
            {
                // Set các semester khác false currentSemester
                var otherSemester = _db.SEMESTERs.Where(x => x.IdSemester != data.IdSemester).ToList();
                foreach (var semester in otherSemester)
                {
                    semester.IsCurrentSemester = false;
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
                    Object = semesterDB.IdSemester,
                };
            }
            catch (Exception ex)
            {
                // Lưu db thất bại
                return new BaseResponse("Thao tác CSDL thất bại, mô tả lỗi từ hệ thống: " + ex.Message);
            }
            finally
            {
                // Tạo năm học cho học kỳ nếu chưa có
                var yearDB = _db.YEARs.Where(x => x.YearName.Equals(data.SemesterYear)).FirstOrDefault();
                if (yearDB is null)
                {
                    yearDB = new YEAR
                    {
                        YearName = data.SemesterYear,
                    };
                    _db.YEARs.Add(yearDB);
                }

                if (!yearDB.SEMESTERs.Any(x => x.IdSemester == semesterDB.IdSemester))
                {
                    yearDB.SEMESTERs.Add(semesterDB);
                }

                // Lưu lại
                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Set học kỳ mặc định cho system
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaseResponse SetCurrentSemester(int id)
        {
            // Kiểm tra dữ liệu có tồn tại không
            var semesterDB = _db.SEMESTERs.Where(x => x.IdSemester == id).FirstOrDefault();
            if (semesterDB is null)
            {
                // Báo lỗi tìm ko thấy
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy dữ liệu này",
                };
            }

            // Set current semester
            semesterDB.IsCurrentSemester = true;

            // Set các semester khác false currentSemester
            var otherSemester = _db.SEMESTERs.Where(x => x.IdSemester != id).ToList();
            foreach (var semester in otherSemester)
            {
                semester.IsCurrentSemester = false;
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
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Thao tác CSDL thất bại, mô tả lỗi từ hệ thống: " + ex.Message,
                };
            }
        }

        /// <summary>
        /// Xoá semester
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaseResponse DeleteSemester(int id)
        {
            // Kiểm tra dữ liệu có tồn tại không
            var semesterDB = _db.SEMESTERs.Where(x => x.IdSemester == id).FirstOrDefault();
            if (semesterDB is null)
            {
                // Báo lỗi tìm ko thấy
                return new BaseResponse("Không tìm thấy dữ liệu này");
            }

            // Không thể xoá học kỳ chính
            if (semesterDB.IsCurrentSemester == true)
            {
                return new BaseResponse("Không thể xoá học kỳ chính");
            }

            // Lưu lại dữ liệu
            try
            {
                // Xoá các dữ liệu liên quan semester
                var idRules = semesterDB.GRADERULEs.ToList().Select(x => x.IdRule).ToList();
                var classesDB = _db.CLASSes.Where(x => idRules.Any(i => i == x.IdRule)).ToList();
                foreach (var classDB in classesDB)
                {
                    classDB.IdRule = null;
                }

                _db.GRADERULEs.RemoveRange(semesterDB.GRADERULEs.ToList());
                _db.TIMELINEs.RemoveRange(semesterDB.TIMELINEs.ToList());
                _db.SCOREs.RemoveRange(semesterDB.SCOREs.ToList());
                _db.SEMESTERRANKs.RemoveRange(semesterDB.SEMESTERRANKs.ToList());

                // Hard delete
                _db.SEMESTERs.Remove(semesterDB);

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

        /// <summary>
        /// Lấy thông tin semester
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BaseResponse GetSemesterById(int id)
        {
            var semesterDB = _db.SEMESTERs.AsNoTracking().Where(x => x.IdSemester == id).FirstOrDefault();

            // Báo lỗi nếu user ko tồn tại
            if (semesterDB is null)
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
                Object = ConvertToViewModel(semesterDB),
            };
        }

        /// <summary>
        /// Lấy toàn bộ học kỳ
        /// </summary>
        public SettingCurrentSemesterResponse GetAll(ViewSemesterTypeEnum viewType = ViewSemesterTypeEnum.ONLY_SEMESTER)
        {
            var listSemesterDB = _db.SEMESTERs
                .AsNoTracking()
                .ToList();
            var currentSemester = _db.SEMESTERs.AsNoTracking().Where(x => x.IsCurrentSemester == true).FirstOrDefault();
            int? selectedSemester = currentSemester?.IdSemester;
            int? idYear = currentSemester?.IdYear;

            // Filter data base on view type
            var listSemester = new List<SemesterViewModel>();
            switch (viewType)
            {
                case ViewSemesterTypeEnum.ONLY_SEMESTER:
                    listSemesterDB = listSemesterDB.ToList();
                    listSemester = listSemesterDB.OrderByDescending(x => x.DateEnd)
                                                .ToList()
                                                .Select(x => ConvertToViewModel(x))
                                                .ToList();
                    break;
                case ViewSemesterTypeEnum.ALL:
                    break;
                case ViewSemesterTypeEnum.ONLY_YEAR:
                    listSemester = _db.YEARs.ToList().Select(x => ConvertToViewModel(x)).ToList();
                    selectedSemester = currentSemester?.IdYear;
                    break;
            }

            return new SettingCurrentSemesterResponse
            {
                Data = listSemester,
                SelectedId = selectedSemester,
                SelectedIdYear = idYear,
                CurrentSemesterName = currentSemester?.SemesterName + " - " + currentSemester?.YEAR?.YearName,
            };
        }

        /// <summary>
        /// Lấy ID học kỳ hiện tại
        /// </summary>
        /// <returns></returns>
        public int? GetCurrentSemesterId()
        {
            var semesterDB = _db.SEMESTERs.Where(x => x.IsCurrentSemester == true).FirstOrDefault();
            return semesterDB?.IdSemester;
        }

        /// <summary>
        /// Lấy năm học hiện tại
        /// </summary>
        /// <returns></returns>
        public int? GetCurrentYearId()
        {
            var semesterDB = _db.SEMESTERs.Where(x => x.IsCurrentSemester == true).FirstOrDefault();
            return semesterDB?.IdYear;
        }

        /// <summary>
        /// Lấy danh sách học kỳ trong năm học
        /// </summary>
        /// <param name="idSemester"></param>
        /// <returns></returns>
        public List<SemesterViewModel> GetSemesterInYear(int idYear)
        {
            return _db.SEMESTERs.AsNoTracking().Where(x => x.IdYear == idYear).ToList()
                .OrderBy(x => x.DateStart)
                .Select(x => ConvertToViewModel(x)).ToList();
        }
    }
}

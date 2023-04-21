using GMAH.Entities;
using GMAH.Models.Models;
using GMAH.Models.ViewModels;
using GMAH.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GMAH.Services.Services
{
    public class TimelineService : BaseService
    {
        /// <summary>
        /// Lấy option xem tkb
        /// </summary>
        /// <param name="idSemester"></param>
        /// <param name="idClass"></param>
        /// <returns></returns>
        public BaseResponse GetTimelineDateRangeViewModel(int idSemester, int idClass)
        {
            var classDB = _db.CLASSes.Where(x => x.IdClass == idClass && x.YEAR.SEMESTERs.Any(i => i.IdSemester == idSemester)).FirstOrDefault();
            if (classDB is null)
            {
                return new BaseResponse("Không tìm thấy lớp học này");
            }

            var minDate = classDB.TIMELINEs.Where(x => x.IdSemester == idSemester).Min(x => x.Date) ?? DateTime.Now;
            var maxDate = classDB.TIMELINEs.Where(x => x.IdSemester == idSemester).Max(x => x.Date) ?? DateTime.Now;

            var minDateMonday = minDate.StartOfWeek(DayOfWeek.Monday);

            var listVM = new List<TimelineDateRangeViewModel>();
            for (var date = minDateMonday.Date; date <= maxDate.Date; date = date.AddDays(7))
            {
                listVM.Add(new TimelineDateRangeViewModel
                {
                    DateFrom = date,
                    DateTo = date.AddDays(6),
                    IsCurrentWeek = date >= DateTime.Now.Date && DateTime.Now.Date <= date.AddDays(6),
                });
            }

            return new BaseResponse
            {
                IsSuccess = true,
                Object = listVM,
            };
        }

        /// <summary>
        /// Show thời khoá biểu
        /// </summary>
        /// <param name="idSemester"></param>
        /// <param name="idClass"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public BaseResponse GetTimeline(int idSemester, int idClass, DateTime from, DateTime to)
        {
            var classDB = _db.CLASSes.Where(x => x.IdClass == idClass && x.YEAR.SEMESTERs.Any(i => i.IdSemester == idSemester)).FirstOrDefault();
            if (classDB is null)
            {
                return new BaseResponse("Không tìm thấy lớp học này");
            }

            from = from.Date;
            to = to.Date;

            var timeLineData = classDB.TIMELINEs.Where(x => x.IdSemester == idSemester && x.Date.HasValue)
                .OrderBy(x => x.Date).ThenBy(x => x.Period)
                .ToList()
                .Where(x => x.Date.Value.Date >= from && x.Date.Value.Date <= to.Date)
                .ToList();
            var listVM = new List<TimelineViewModel>();

            for (var date = from.Date; date <= to.Date; date = date.AddDays(1))
            {
                var vm = listVM.Where(x => x.Date == date.Date).FirstOrDefault();
                if (vm is null)
                {
                    vm = new TimelineViewModel
                    {
                        Date = date,
                        DateString = date.ToString("dd/MM"),
                        DayOfWeek = date.DateOfWeekString(),
                        Detail = new List<TimelineDetailViewModel>(),
                    };
                    listVM.Add(vm);
                }

                var data = timeLineData.Where(x => x.Date.HasValue && x.Date.Value.Date == date.Date).ToList();
                foreach (var subject in data)
                {
                    vm.Detail.Add(new TimelineDetailViewModel
                    {
                        Period = subject.Period ?? -1,
                        SubjectName = subject.SUBJECT.SubjectName,
                        TeacherFullname = subject.SUBJECT.CLASS_SUBJECT.Where(x => x.IdClass == idClass).FirstOrDefault()?.TEACHER_SUBJECT?.TEACHER?.USER?.Fullname ?? String.Empty,
                    });
                }
            }

            return new BaseResponse
            {
                IsSuccess = true,
                Object = listVM,
            };
        }

        /// <summary>
        /// Nhập thời khoá biểu từ file excel
        /// </summary>
        public BaseResponse ImportTimeline(int idClass, int idSemester, List<TimelineData> timelineData)
        {
            var classDB = _db.CLASSes.Where(x => x.IdClass == idClass && x.YEAR.SEMESTERs.Any(i => i.IdSemester == idSemester)).FirstOrDefault();
            if (classDB is null)
            {
                return new BaseResponse("Không tìm thấy lớp học này");
            }

            // Biến mã môn thành id
            foreach (var data in timelineData)
            {
                data.IdSubject = _db.SUBJECTs.Where(x => x.SubjectCode.Equals(data.SubjectCode)).FirstOrDefault()?.IdSubject ?? -1;
            }

            // Lấy danh sách môn học
            var listIdSubject = timelineData.Select(x => x.IdSubject).ToList();

            // Kiểm tra danh sách môn học
            var listClassIdSubject = classDB.CLASS_SUBJECT.Select(x => x.IdSubject).ToList();
            if (listIdSubject.Any(x => !listClassIdSubject.Any(i => i == x)))
            {
                return new BaseResponse("Vui lòng kiểm tra lại các môn học mà bạn nhập, đảm bảo rằng môn học này được thiết lập dạy cho lớp");
            }

            // Import data
            foreach (var data in timelineData)
            {
                for (var date = data.DateFrom.Date; date <= data.DateTo.Date; date = date.AddDays(1))
                {
                    if (date.Date.DayOfWeek != data.DayOfWeek) continue;

                    foreach (var period in data.Periods)
                    {
                        _db.TIMELINEs.Add(new TIMELINE
                        {
                            IdSchedule = 0,
                            IdClass = idClass,
                            IdSemester = idSemester,
                            IdSubject = data.IdSubject,
                            Date = date,
                            Period = period,
                        });
                    }
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
        /// Xoá hết thời gian biểu cũ
        /// </summary>
        /// <param name="idSemester"></param>
        /// <param name="idClass"></param>
        public BaseResponse ClearTimeline(int idSemester, int idClass)
        {
            var classDB = _db.CLASSes.Where(x => x.IdClass == idClass && x.YEAR.SEMESTERs.Any(i => i.IdSemester == idSemester)).FirstOrDefault();
            if (classDB is null)
            {
                return new BaseResponse("Không tìm thấy lớp học này");
            }

            foreach (var item in classDB.TIMELINEs.ToList())
            {
                _db.TIMELINEs.Remove(item);
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
    }
}

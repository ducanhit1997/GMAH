using GMAH.Entities;
using GMAH.Models.Consts;
using GMAH.Models.ViewModels;
using System;
using System.Linq;

namespace GMAH.Services.Services
{
    public class BehaviourService : BaseService
    {
        /// <summary>
        /// Đánh giá hạnh kiểm của học sinh
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="idSemester"></param>
        /// <param name="rank"></param>
        /// <returns></returns>
        public BaseResponse SetBehaviourRank(int idUser, int? idSemester, int idYear, int? rank)
        {
            // Kiểm tra học sinh
            var studentDB = _db.USERs.Where(x => x.IdUser == idUser && x.IdRole == (int)RoleEnum.STUDENT && x.IsDeleted != true).FirstOrDefault()?.STUDENTs.FirstOrDefault();
            if (studentDB is null)
            {
                return new BaseResponse("Học sinh này không tồn tại");
            }

            // Lấy semester
            SEMESTERRANK semesterRankDB = null;
            if (idSemester != null)
            {
                semesterRankDB = _db.SEMESTERRANKs.Where(x => x.IdSemester == idSemester && x.STUDENT_CLASS.IdStudent == studentDB.IdStudent).FirstOrDefault();
            }
            else
            {
                semesterRankDB = _db.SEMESTERRANKs.Where(x => x.IdYear == idYear && x.STUDENT_CLASS.IdStudent == studentDB.IdStudent && x.IdSemester == null).FirstOrDefault();
            }

            // Ko tồn tại thì tạo mới
            if (semesterRankDB is null)
            {
                // Lấy id student class
                var studentClassDB = studentDB.STUDENT_CLASS.Where(x => x.CLASS.IdYear == idYear).FirstOrDefault();

                if (studentClassDB is null)
                {
                    return new BaseResponse("Học sinh này không có lớp trong học kỳ bạn chọn");
                }

                int idStudentClass = studentClassDB.IdStudentClass;

                semesterRankDB = new SEMESTERRANK
                {
                    IdSemester = idSemester,
                    IdYear = idYear,
                    IdStudentClass = idStudentClass,
                };

                _db.SEMESTERRANKs.Add(semesterRankDB);
            }

            // Đổi hạnh kiểm
            semesterRankDB.IdBehaviour = rank;

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
            finally
            {
                var scoreSemesterService = new ScoreSemesterService();
                scoreSemesterService.CalculateSubjectAvgSingleStudent(idUser, idSemester, idYear);
            }
        }
    }
}

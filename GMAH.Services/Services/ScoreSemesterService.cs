using GMAH.Entities;
using GMAH.Models.Consts;
using GMAH.Models.Models;
using GMAH.Models.ViewModels;
using GMAH.Services.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;

namespace GMAH.Services.Services
{
    /// <summary>
    /// Sử dụng chủ yếu cho con job
    /// Cập nhật điểmh học kỳ,...
    /// </summary>
    public class ScoreSemesterService : BaseService
    {
        private SystemSettingService settingService;

        public ScoreSemesterService() : base()
        {
            settingService = new SystemSettingService();
        }

        /// <summary>
        /// Cập nhật điểm cho 1 học sinh
        /// </summary>
        public void CalculateSubjectAvgSingleStudent(int idUser, int? idSemester, int idYear)
        {
            // Lấy student
            var studentInSemester = _db.STUDENT_CLASS.Where(x => x.STUDENT.IdUser == idUser && x.CLASS.IdYear == idYear).ToList();

            // Duyệt từng student để update
            foreach (var student in studentInSemester)
            {
                var resultUpdateAvgSubject = UpdateAvgSubjectForStudent(student, idSemester, idYear);

                // Nếu update điểm đầy đủ thì update qua điểm học kỳ và xếp hạng học kỳ
                if (resultUpdateAvgSubject.CanUpdateSemesterScore)
                {
                    // Cho từng học kỳ
                    UpdateAvgScoreSemester(student, resultUpdateAvgSubject, idYear);

                    // Kiểm tra và update điểm toàn học kỳ
                    UpdateAvgScoreYear(student, idYear, resultUpdateAvgSubject.Scores.Select(x => x.IdSubject).ToList());
                }
            }
        }

        /// <summary>
        /// Cập nhật điểm trung bình môn
        /// </summary>
        public void CalculateSubjectAvgScoreSemester()
        {
            // Lấy học kỳ hiện tại
            var currentSemester = _db.SEMESTERs.Where(x => x.IsCurrentSemester == true).FirstOrDefault();

            // Nếu ko có học kỳ hiện tại thì ko process tiếp
            if (currentSemester is null || currentSemester.YEAR is null)
            {
                return;
            }

            // Lấy lớp trong semester
            var classInSemester = currentSemester.YEAR.CLASSes;

            // Lấy toàn bộ student
            var studentInSemester = classInSemester.SelectMany(x => x.STUDENT_CLASS).ToList();

            // Lấy các công thức tính điểm
            var idYear = currentSemester.IdYear ?? 0;

            // Duyệt từng student để update
            foreach (var student in studentInSemester)
            {
                var resultUpdateAvgSubject = UpdateAvgSubjectForStudent(student, currentSemester.IdSemester, currentSemester.IdYear ?? -1);

                // Nếu update điểm đầy đủ thì update qua điểm học kỳ và xếp hạng học kỳ
                if (resultUpdateAvgSubject.CanUpdateSemesterScore)
                {
                    // Cho từng học kỳ
                    UpdateAvgScoreSemester(student, resultUpdateAvgSubject, idYear);

                    // Kiểm tra và update điểm toàn học kỳ
                    UpdateAvgScoreYear(student, idYear, resultUpdateAvgSubject.Scores.Select(x => x.IdSubject).ToList());
                }
            }
        }

        /// <summary>
        /// Cập nhật điểm cho năm học
        /// </summary>
        /// <param name="student"></param>
        /// <param name="idYear"></param>
        /// <param name="idSubjects"></param>
        public void UpdateAvgScoreYear(STUDENT_CLASS student, int idYear, List<int> idSubjects)
        {
            // Lấy thông tin năm học
            var yearDB = _db.YEARs.Where(x => x.IdYear == idYear).FirstOrDefault();

            // Nếu dữ liệu null thì nghỉ
            if (yearDB is null)
            {
                return;
            }

            // Kiểm tra dữ liệu của từng năm học đã đầy đủ chưa
            foreach (var semester in yearDB.SEMESTERs)
            {
                var scoreDB = student.SEMESTERRANKs.Where(x => x.IdSemester == semester.IdSemester).FirstOrDefault();

                // Nếu dữ liệu thiếu hay ko đầy đủ thì dừng ngay lập tức
                if (scoreDB?.AvgScore is null)
                {
                    return;
                }
            }

            // Tính điểm cuối năm cho từng môn học
            var subjectScore = new List<AvgSubjectForStudentData>();
            foreach (var idSubject in idSubjects)
            {
                double avgSubjectScoreInYear = 0;

                foreach (var semester in yearDB.SEMESTERs)
                {
                    var scoreDB = student.SCOREs.Where(x => x.IdSemester == semester.IdSemester && x.IdSubject == idSubject).FirstOrDefault();

                    // Nếu điểm thành phần thì nghỉ
                    if (scoreDB is null)
                    {
                        return;
                    }

                    // Cộng điểm
                    avgSubjectScoreInYear += (scoreDB.Score1 ?? 0) * (semester.ScoreWeight ?? 0) /100;
                }

                // Làm tròn đến 1 chữ số
                avgSubjectScoreInYear = Math.Round(avgSubjectScoreInYear, 1);

                // Lưu vào db
                var subjectScoreYear = _db.SCOREs.Where(x => x.IdSubject == idSubject && x.IdStudentClass == student.IdStudentClass && x.IdYear == idYear && x.IdSemester == null).FirstOrDefault();
                if (subjectScoreYear is null)
                {
                    subjectScoreYear = new SCORE
                    {
                        IdYear = idYear,
                        IdStudentClass = student.IdStudentClass,
                        IdSubject = idSubject,
                    };

                    _db.SCOREs.Add(subjectScoreYear);
                }

                subjectScoreYear.Score1 = avgSubjectScoreInYear;
                _db.SaveChanges();

                // Lưu vào array
                subjectScore.Add(new AvgSubjectForStudentData
                {
                    IdSubject = idSubject,
                    Score = avgSubjectScoreInYear,
                });
            }

            // Tính điểm cả năm
            var avgInYearDB = _db.SEMESTERRANKs.Where(x => x.IdYear == idYear && x.IdStudentClass == student.IdStudentClass && x.IdSemester == null).FirstOrDefault();
            if (avgInYearDB is null)
            {
                avgInYearDB = new SEMESTERRANK
                {
                    IdYear = idYear,
                    IdStudentClass = student.IdStudentClass,
                };

                _db.SEMESTERRANKs.Add(avgInYearDB);
            }

            // Nạp điểm
            var avgScore = subjectScore.Select(x => x.Score).Average();
            avgScore = Math.Round(avgScore, 1);
            avgInYearDB.AvgScore = avgScore;

            // Xếp loại
            var idClass = student.CLASS.IdClass;
            var ruleDB = _db.GRADERULEs.Where(x => x.IdSemester == idYear && x.CLASSes.Any(i => i.IdClass == idClass)).FirstOrDefault();
            avgInYearDB.IdRank = (int?)CalculateRank(ruleDB, subjectScore, avgInYearDB.IdBehaviour);

            _db.SaveChanges();
        }

        /// <summary>
        /// Cập nhật điểm cho môn học
        /// </summary>
        public UpdateAvgSubjectForStudentResult UpdateAvgSubjectForStudent(STUDENT_CLASS student, int? idSemester, int idYear)
        {
            // Đánh dấu học sinh đủ điều kiện để update điểm học kỳ
            bool updateSemesterAvgScore = true;
            var listScore = new List<AvgSubjectForStudentData>();
            var idBehaviour = student.SEMESTERRANKs.Where(x => x.IdSemester == idSemester).FirstOrDefault()?.IdBehaviour;

            // Lấy danh sách môn học cần update
            var listSubject = student.CLASS.CLASS_SUBJECT.ToList();
            foreach (var subject in listSubject)
            {
                // Kiểm tra có điểm hay chưa, nếu chưa thì ko cần update
                var scoreDBs = student.SCOREs.Where(x => x.IdSubject == subject.IdSubject &&  
                     ((idSemester != null && x.IdSemester == idSemester) || (idSemester == null && x.IdYear == idYear && x.IdSemester == null))
                ).ToList();
                if (scoreDBs.Count < 1)
                {
                    updateSemesterAvgScore = false;
                    continue;
                }

                // Nếu chưa có điểm cuối kỳ thì ko cần phải cập nhật
                if (scoreDBs.Any(x => x.Score1 == null))
                {
                    updateSemesterAvgScore = false;
                    continue;
                }

                // Tính công thức
                double avgScore = 0;

                // Follow theo https://hoatieu.vn/phap-luat/cach-tinh-diem-trung-binh-mon-hoc-ky-nam-hoc-203433
                if (idSemester != null)
                {
                    int total = scoreDBs.Sum(x => (x.SCORE_TYPE?.ScoreWeight ?? 0));
                    double score = scoreDBs.Sum(x => (x.Score1 ?? 0) * (x.SCORE_TYPE?.ScoreWeight ?? 0));
                    avgScore = score / total;
                }
                else
                {
                    avgScore = scoreDBs.FirstOrDefault()?.Score1 ?? 0;
                }
      

                try
                {
                    avgScore = Math.Round(avgScore, 1);
                    listScore.Add(new AvgSubjectForStudentData
                    {
                        IdSubject = subject.IdSubject,
                        Score = avgScore,
                    });

                    // Lấy điểm học kỳ hoặc năm học
                    var scoreSemesterDB = scoreDBs.Where(x => x.IdScoreType == null).FirstOrDefault();
                    if (scoreSemesterDB is null)
                    {
                        // Kiểm tra xem có cột điểm trung bình ko
                        scoreSemesterDB = new SCORE
                        {
                            IdScoreType = null,
                            IdSemester = idSemester,
                            IdStudentClass = student.IdStudentClass,
                            IdSubject = subject.IdSubject,
                            IdYear = student.CLASS.IdYear,
                        };

                        _db.SCOREs.Add(scoreSemesterDB);
                    }
                    scoreSemesterDB.Score1 = avgScore;

                    _db.SaveChanges();
                }
                catch
                {
                    // Nếu có lỗi thì đi tiếp
                    updateSemesterAvgScore = false;
                    continue;
                }
            }

            return new UpdateAvgSubjectForStudentResult
            {
                IdBehaviour = idBehaviour,
                IdSemester = idSemester,
                IdYear = idYear,
                CanUpdateSemesterScore = updateSemesterAvgScore,
                Scores = listScore,
            };
        }

        /// <summary>
        /// Cập nhật điểm học kỳ
        /// </summary>
        public void UpdateAvgScoreSemester(STUDENT_CLASS student, UpdateAvgSubjectForStudentResult data, int idYear)
        {
            // Lấy điểm tb học kỳ
            var semesterRankDB = _db.SEMESTERRANKs.Where(x => x.IdSemester == data.IdSemester && x.IdStudentClass == student.IdStudentClass).FirstOrDefault();
            
            // Ko có thì tạo mới
            if (semesterRankDB is null)
            {
                semesterRankDB = new SEMESTERRANK
                {
                    IdYear = idYear,
                    IdSemester = data.IdSemester,
                    IdStudentClass = student.IdStudentClass,
                };

                _db.SEMESTERRANKs.Add(semesterRankDB);
            }

            // Nạp dữ liệu
            var avgScore = data.Scores.Select(x => x.Score).Average();
            avgScore = Math.Round(avgScore, 1);
            semesterRankDB.AvgScore = avgScore;

            // Dựa vào luật, tính toán xếp hạng
            var idClass = student.CLASS.IdClass;
            var ruleDB = _db.GRADERULEs.Where(x => x.IdSemester == idYear && x.CLASSes.Any(i => i.IdClass == idClass)).FirstOrDefault();
            semesterRankDB.IdRank = (int?)CalculateRank(ruleDB, data.Scores, semesterRankDB.IdBehaviour);

            // Lưu dữ liệu
            _db.SaveChanges();
        }

        /// <summary>
        /// Xếp hạng cho học sinh
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="subject"></param>
        public RankEnum? CalculateRank(GRADERULE rule, List<AvgSubjectForStudentData> subject, int? idBehaviour)
        {
            // Nếu ko có rule thì ko xếp hạng đc
            if (rule is null || subject is null)
            {
                return null;
            }

            // Hạnh kiểm ko có thì ko xếp hạng
            if (idBehaviour is null)
            {
                return null;
            }

            // Trung bình học kỳ
            var avgScore = subject.Select(x => x.Score).Average();
            avgScore = Math.Round(avgScore, 1);

            // Đi từ hạng cao đến hạng thấp, thoả mãn hạng nào dừng tại đó
            foreach (RankEnum grade in Enum.GetValues(typeof(RankEnum)))
            {
                var gradeRule = rule.GRADERULELISTs.Where(x => x.IdRank == (int)grade).First();

                // Kiểm tra TB học kỳ trước
                if (avgScore < (gradeRule.MinAvgScore ?? 0))
                {
                    // Ko thoả thì đi tiếp hạng thấp hơn
                    continue;
                }

                // Kiểm tra đến hạnh kiểm
                if (idBehaviour.Value > (gradeRule.IdBehaviour ?? 0))
                {
                    // Ko thoả thì đi tiếp hạng thấp hơn
                    continue;
                }

                // Kiểm tra từng tiêu chí avg score subject
                bool meetCriteria = true;
                foreach (var ruleDetail in gradeRule.GRADERULEDETAILs)
                {
                    if (ruleDetail.IdSubject is null)
                    {
                        // Kiểm tra toàn bộ điểm toàn môn
                        meetCriteria = !subject.Select(x => x.Score).Any(x => x < ruleDetail.MinAvgScore);
                    }
                    else
                    {
                        // Kiểm tra 1 điểm nào đó
                        var scoreSubject = subject.Where(x => x.IdSubject == ruleDetail.IdSubject).FirstOrDefault();
                        if (scoreSubject is null)
                        {
                            meetCriteria = false;
                        }
                        else
                        {
                            meetCriteria = scoreSubject.Score >= ruleDetail.MinAvgScore;
                        }
                    }

                    // Nếu ko thoả một tiêu chí nào đó thì ngừng
                    if (!meetCriteria)
                    {
                        break;
                    }
                }

                // Nếu thoả toàn bộ tiêu chí thì trả về hạng hiện tại
                if (meetCriteria)
                {
                    return grade;
                }
            }

            // Ko xếp hạng đc
            return null;
        }

        /// <summary>
        /// Lấy xếp hạng học kỳ
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="idSemester"></param>
        /// <returns></returns>
        public ScoreSubjectViewModel GetSemesterRank(int idUser, int? idSemester, int idYear)
        {
            var result = new ScoreSubjectViewModel
            {
                Details = new List<ScoreDetailViewModel>(),
            };

            // Lấy semester
            var semesterRank = _db.SEMESTERRANKs.Where(x => x.IdYear == idYear && (x.IdSemester == idSemester) && x.STUDENT_CLASS.STUDENT.IdUser == idUser).FirstOrDefault();
            if (semesterRank is null)
            {
                return null;
            }

            // Lấy result
            result.SubjectName = semesterRank.IdSemester is null ? "Cả năm" : semesterRank.SEMESTER.SemesterName;
            result.YearName = semesterRank.YEAR.YearName;
            result.StudentName = semesterRank.STUDENT_CLASS.STUDENT.USER.Fullname;

            // Lấy điểm thành phần
            result.Details.Add(new ScoreDetailViewModel
            {
                ScoreName = "Score",
                Text = semesterRank.AvgScore.ToString(),
            });

            result.Details.Add(new ScoreDetailViewModel
            {
                ScoreName = "Behaviour",
                Text = semesterRank.IdBehaviour.HasValue ? ((BehaviourEnum)semesterRank.IdBehaviour.Value).GetAttribute<DisplayAttribute>().Name : "",
            });

            result.Details.Add(new ScoreDetailViewModel
            {
                ScoreName = "Rank",
                Text = semesterRank.IdRank.HasValue ? ((RankEnum)semesterRank.IdRank.Value).GetAttribute<DisplayAttribute>().Name : "",
            });

            return result;
        }

        /// <summary>
        /// Gửi mail score cho phụ huynh
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public BaseResponse SendMailScore(string filePath, int idUser, string content = null)
        {
            var userDB = _db.USERs.Where(x => x.IdUser == idUser && x.IsDeleted != true).FirstOrDefault()?.STUDENTs.FirstOrDefault();

            // Báo lỗi nếu user ko tồn tại
            if (userDB is null)
            {
                return new BaseResponse("Học sinh không tồn tại");
            }

            // Kiểm tra user có mail ko
            if (!userDB.USERs.Any())
            {
                return new BaseResponse("Học sinh này không có phụ huynh");
            }

            // Kiểm tra list mail của phụ huynh
            var listEmail = userDB.USERs.Select(x => x.Email).ToList();
            if (listEmail.Any(x => string.IsNullOrEmpty(x)))
            {
                return new BaseResponse("Phụ huynh của học sinh không có email, vui lòng kiểm tra lại");
            }

            // Lấy thông tin email
            var settingService = new SystemSettingService();
            var emailUsername = settingService.GetSetting(SettingEnum.EMAIL_USERNAME);
            var emailPassword = settingService.GetSetting(SettingEnum.EMAIL_PASSWORD);
            var senderName = settingService.GetSetting(SettingEnum.EMAIL_SENDERNAME);
            var smtp = settingService.GetSetting(SettingEnum.EMAIL_SMTP);
            var port = settingService.GetSetting(SettingEnum.EMAIL_PORT);

            // Nếu setting ko có info thì báo lỗi
            if (string.IsNullOrEmpty(emailUsername) ||
                string.IsNullOrEmpty(emailPassword) ||
                string.IsNullOrEmpty(senderName) ||
                string.IsNullOrEmpty(smtp) ||
                string.IsNullOrEmpty(port))
            {
                return new BaseResponse("Thiết lập hệ thống không đầy đủ, vui lòng liên hệ quản trị viên");
            }

            // Send mail
            var emailUtility = new EmailUtility(senderName, emailUsername, emailPassword, smtp, port);
            foreach (var email in listEmail)
            {
                if (!string.IsNullOrEmpty(content))
                {
                    emailUtility.Send(email, "BÁO CÁO ĐIỂM HỌC KỲ", content);
                }
                else
                {
                    emailUtility.Send(email, "BÁO CÁO ĐIỂM HỌC KỲ", $"Vui lòng xem tệp đính kèm", filePath);
                }
            }

            return new BaseResponse
            {
                IsSuccess = true,
            };
        }
    }
}

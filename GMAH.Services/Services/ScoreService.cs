using GMAH.Entities;
using GMAH.Models.Consts;
using GMAH.Models.Models;
using GMAH.Models.ViewModels;
using GMAH.Services.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace GMAH.Services.Services
{
    /// <summary>
    /// Xử lý business điểm số
    /// </summary>
    public class ScoreService : BaseService
    {
        /// <summary>
        /// Lấy điểm của 1 học sinh
        /// </summary>
        public GetClassScoreResponse ParentGetChildScore(int? idParent, int idChild, int idSemester, ScoreViewTypeEnum viewType = ScoreViewTypeEnum.DETAIL)
        {
            // Lấy lớp và kiểm tra dữ liệu tồn tại
            var studentDB = _db.STUDENTs.Where(x => x.IdUser == idChild && (x.USERs.Any(i => i.IdUser == idParent) || idChild == idParent)).FirstOrDefault();

            if (studentDB is null)
            {
                return new GetClassScoreResponse
                {
                    IsSuccess = false,
                    Message = "Học sinh này không tồn tại",
                };
            }

            // Kiểm tra semester
            var classDB = studentDB.STUDENT_CLASS.Where(x => x.CLASS.YEAR.SEMESTERs.Any(i => i.IdSemester == idSemester)).Select(x => x.CLASS).FirstOrDefault();
            if (classDB is null)
            {
                return new GetClassScoreResponse
                {
                    IsSuccess = false,
                    Message = "Học sinh không có điểm trong học kỳ này",
                };
            }

            // Tuỳ vào role mà đc quyền xem những cột điểm nào
            var idSubjectCanView = classDB.CLASS_SUBJECT.Select(x => x.IdSubject).ToList();

            var listStudentScore = new List<ScoreViewModel>();
            var semesterYearName = classDB.YEAR.YearName;

            switch (viewType)
            {
                case ScoreViewTypeEnum.DETAIL:
                    listStudentScore.Add(GetStudentScoreBySemester(idChild, idSemester, idSubjectCanView));
                    break;
                case ScoreViewTypeEnum.AVG:
                    listStudentScore.Add(GetStudentAvgScoreSubjectByYear(idChild, classDB.IdYear ?? -1, idSubjectCanView));
                    break;
                default:
                    break;
            }

            // Lấy view model để render column
            var columns = new List<ScoreComponentViewModel>();
            foreach (var subject in listStudentScore.SelectMany(x => x.Subjects).Select(x => x.SubjectName).Distinct().ToList())
            {
                var column = new ScoreComponentViewModel
                {
                    SubjectName = subject,
                    Column = listStudentScore.SelectMany(x => x.Subjects).Where(x => x.SubjectName.Equals(subject)).SelectMany(x => x.Details).Select(x => x.ScoreName).Distinct().ToList(),
                    ColumnId = listStudentScore.SelectMany(x => x.Subjects).Where(x => x.SubjectName.Equals(subject)).SelectMany(x => x.Details).Select(x => x.IdScoreType).Distinct().ToList(),
                };

                if (column.Column.Count == 0) continue;
                columns.Add(column);
            }

            // Trả về dữ liệu
            return new GetClassScoreResponse
            {
                IdSemester = idSemester,
                IdYear = classDB.IdYear ?? -1,
                ScoreComponent = columns,
                IsSuccess = true,
                Object = listStudentScore,
            };
        }

        /// <summary>
        /// Lấy điểm của 1 lớp
        /// </summary>
        /// <param name="idClass"></param>
        /// <param name="idSemester"></param>
        public GetClassScoreResponse GetClassScoreBySemester(int? idAdmin, int idClass, int idSemester, ScoreViewTypeEnum viewType = ScoreViewTypeEnum.DETAIL)
        {
            // Lấy lớp và kiểm tra dữ liệu tồn tại
            var classDB = _db.CLASSes.Where(x => x.IdClass == idClass).FirstOrDefault();

            if (classDB is null)
            {
                return new GetClassScoreResponse
                {
                    IsSuccess = false,
                    Message = "Lớp học này không tồn tại",
                };
            }

            // Kiểm tra semester có nằm trong lớp này hay ko
            if (!classDB.YEAR.SEMESTERs.Any(x => x.IdSemester == idSemester))
            {
                return new GetClassScoreResponse
                {
                    IsSuccess = false,
                    Message = "Học kỳ không nằm trong năm học của lớp",
                };
            }

            // Tuỳ vào role mà đc quyền xem những cột điểm nào
            var idSubjectCanView = classDB.CLASS_SUBJECT.Select(x => x.IdSubject).ToList();
            var allowEditBehaviour = false;

            if (idAdmin != null)
            {
                var adminDB = _db.USERs.AsNoTracking().Where(x => x.IdUser == idAdmin).FirstOrDefault();
                if (adminDB is null)
                {
                    return new GetClassScoreResponse
                    {
                        IsSuccess = false,
                        Message = "Người dùng không tồn tại",
                    };
                }

                switch ((RoleEnum)adminDB.IdRole)
                {
                    // Hai role cao nhất được xem toàn bộ
                    case RoleEnum.MANAGER:
                    case RoleEnum.ASSISTANT:
                        allowEditBehaviour = true;
                        break;

                    case RoleEnum.HEAD_OF_SUBJECT:
                    case RoleEnum.TEACHER:
                        // Nếu là role của giáo viên chủ nhiệm thì kiểm tra GVCN đúng lớp không
                        if (classDB.IdFormTeacher.HasValue && classDB.TEACHER.USER.IdUser == idAdmin)
                        {
                            allowEditBehaviour = true;
                            break;
                        }
                        // Nếu là giáo viên thì phải là giáo viên đang giảng dạy bộ môn đó
                        else
                        {
                            // Danh sách môn học mầ giáo viên này giảng dạy
                            var idTeacher = adminDB.TEACHERs.First().IdTeacher;
                            idSubjectCanView = classDB.CLASS_SUBJECT.Where(x => x.TEACHER_SUBJECT != null && x.TEACHER_SUBJECT.IdTeacher == idTeacher).Select(x => x.IdSubject).ToList();
                        }
                        break;

                    // Nếu ko thuộc trường hợp nào thì ko cho xem
                    default:
                        idSubjectCanView.Clear();
                        break;
                }
            }

            // Lấy danh sách lớp, lấy theo id user
            var allIdUserOfStudent = classDB.STUDENT_CLASS.Select(x => x.STUDENT.USER.IdUser).ToList();

            var listStudentScore = new List<ScoreViewModel>();
            var semesterYearName = classDB.YEAR.YearName;
            foreach (var idUser in allIdUserOfStudent)
            {
                switch (viewType)
                {
                    case ScoreViewTypeEnum.DETAIL:
                        listStudentScore.Add(GetStudentScoreBySemester(idUser, idSemester, idSubjectCanView));
                        break;
                    case ScoreViewTypeEnum.AVG:
                        var avgData = GetStudentAvgScoreSubjectByYear(idUser, classDB.IdYear ?? -1, idSubjectCanView, allowEditBehaviour);
                        listStudentScore.Add(avgData);
                        break;
                    default:
                        break;
                }
            }

            // Lấy view model để render column
            var columns = new List<ScoreComponentViewModel>();
            foreach (var subject in listStudentScore.SelectMany(x => x.Subjects).Select(x => x.SubjectName).Distinct().ToList())
            {
                var column = new ScoreComponentViewModel
                {
                    SubjectName = subject,
                    Column = listStudentScore.SelectMany(x => x.Subjects).Where(x => x.SubjectName.Equals(subject)).SelectMany(x => x.Details).Select(x => x.ScoreName).Distinct().ToList(),
                    ColumnId = listStudentScore.SelectMany(x => x.Subjects).Where(x => x.SubjectName.Equals(subject)).SelectMany(x => x.Details).Select(x => x.IdScoreType).Distinct().ToList(),
                };

                if (column.Column.Count == 0) continue;
                columns.Add(column);
            }

            // Trả về dữ liệu
            return new GetClassScoreResponse
            {
                IdSemester = idSemester,
                IdYear = classDB.IdYear ?? -1,
                ScoreComponent = columns,
                IsSuccess = true,
                Object = listStudentScore,
            };
        }

        /// <summary>
        /// Lấy điểm của 1 học sinh
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="idSemester"></param>
        public ScoreViewModel GetStudentScoreBySemester(int idUser, int idSemester, List<int> idSubjectCanView)
        {
            var result = new ScoreViewModel();

            // Lấy thông tin học sinh
            var studentDB = _db.STUDENTs.Where(x => x.IdUser == idUser).FirstOrDefault();
            if (studentDB is null)
            {
                return result;
            }

            result.StudentName = studentDB.USER.Fullname;
            result.IdUser = studentDB.USER.IdUser;
            result.StudentCode = studentDB.StudentCode;

            // Lấy toàn bộ điểm của học sinh
            var allScore = _db.SCOREs.Where(x => x.STUDENT_CLASS.IdStudent == studentDB.IdStudent && x.IdSemester == idSemester).ToList();

            // Lấy danh sách môn học của lớp học
            var classDB = _db.CLASSes.Where(x => (x.YEAR.SEMESTERs.Any(i => i.IdSemester == idSemester)) && x.STUDENT_CLASS.Any(i => i.STUDENT.IdStudent == studentDB.IdStudent)).FirstOrDefault();
            var allSubjectInClass = classDB?.CLASS_SUBJECT.Select(x => x.SUBJECT).ToList();

            // Nếu lớp học ko có môn học thì return list rỗng
            if (allSubjectInClass is null)
            {
                return result;
            }

            // Nạp TBHK và xếp hạng học kỳ
            var studentClassDB = classDB.STUDENT_CLASS.Where(x => x.IdStudent == studentDB.IdStudent).First();
            var semesterRank = studentClassDB.SEMESTERRANKs.Where(x => x.IdSemester == idSemester).FirstOrDefault();
            result.StudentAvgScore = semesterRank?.AvgScore;
            if (semesterRank?.IdRank != null)
            {
                result.StudentRankName = ((RankEnum)semesterRank.IdRank).GetAttribute<DisplayAttribute>().Name;
            }

            // Kiểm tra giới hạn xem theo môn học
            if (idSubjectCanView != null)
            {
                allSubjectInClass = allSubjectInClass.Where(x => idSubjectCanView.Any(i => i == x.IdSubject)).ToList();
            }

            // Nạp điểm từng môn học
            result.Subjects = new List<ScoreSubjectViewModel>();
            foreach (var subject in allSubjectInClass)
            {
                var scoreInfo = new ScoreSubjectViewModel
                {
                    SubjectName = subject.SubjectName,
                    IdSubject = subject.IdSubject,
                    Details = new List<ScoreDetailViewModel>(),
                };

                // Lấy điểm thành phần
                var scoreDB = allScore.Where(x => x.IdSubject == subject.IdSubject && x.IdScoreType != null).ToList();

                #region Lấy điểm thành phần
                // Thêm các điểm thành phần
                foreach (var scoreItem in scoreDB)
                {
                    scoreInfo.Details.Add(new ScoreDetailViewModel
                    {
                        IdScore = scoreItem.IdScore,
                        IdScoreType = scoreItem.IdScoreType ?? 0,
                        ScoreWeight = scoreItem.SCORE_TYPE?.ScoreWeight ?? 0,
                        ScoreName = scoreItem.SCORE_TYPE?.ScoreName ?? "ĐTB",
                        Score = scoreItem?.Score1,
                        IsReadOnly = scoreItem.IdScoreType is null,
                        Note = scoreItem?.ScoreNote,
                    });
                }

                // Lấy các điểm thành phần khác chưa có trong dữ liệu của học sinh
                var scoreNotExist = _db.SCORE_TYPE.AsNoTracking().Where(x => x.CLASS_SUBJECT.IdClass == classDB.IdClass && x.CLASS_SUBJECT.IdSubject == subject.IdSubject).ToList();
                scoreNotExist = scoreNotExist.Where(x => !scoreDB.Any(i => i.IdScoreType == x.IdScoreType)).ToList();
                foreach (var scoreItem in scoreNotExist)
                {
                    scoreInfo.Details.Add(new ScoreDetailViewModel
                    {
                        IdScore = null,
                        ScoreWeight = scoreItem.ScoreWeight ?? 0,
                        IdScoreType = scoreItem.IdScoreType,
                        ScoreName = scoreItem.ScoreName,
                        Score = null,
                    });
                }

                // Sort lại
                scoreInfo.Details = scoreInfo.Details.OrderBy(x => x.ScoreWeight).ThenByDescending(x => x.IdScoreType).ToList();
                #endregion

                result.Subjects.Add(scoreInfo);
            }

            // Trả dữ liệu
            return result;
        }

        /// <summary>
        /// Lấy điểm môn học theo học kỳ
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="yearName"></param>
        /// <returns></returns>
        public ScoreViewModel GetStudentAvgScoreSubjectByYear(int idUser, int idYear, List<int> idSubjectCanView, bool allowEditBehaviour = true)
        {
            var result = new ScoreViewModel();

            // Lấy thông tin học sinh
            var studentDB = _db.STUDENTs.Where(x => x.IdUser == idUser).FirstOrDefault();
            if (studentDB is null)
            {
                return result;
            }

            result.StudentName = studentDB.USER.Fullname;
            result.IdUser = studentDB.USER.IdUser;
            result.StudentCode = studentDB.StudentCode;

            // Lấy ds môn học
            var classDB = _db.CLASSes.Where(x => x.IdYear == idYear && x.STUDENT_CLASS.Any(i => i.STUDENT.IdStudent == studentDB.IdStudent)).FirstOrDefault();
            var allSubjectInClass = classDB?.CLASS_SUBJECT.Select(x => x.SUBJECT).ToList();

            // Nếu lớp học ko có môn học thì return list rỗng
            if (allSubjectInClass is null)
            {
                return result;
            }

            // Kiểm tra giới hạn xem theo môn học
            if (idSubjectCanView != null)
            {
                allSubjectInClass = allSubjectInClass.Where(x => idSubjectCanView.Any(i => i == x.IdSubject)).ToList();
            }

            // Lấy học kỳ
            var semesterInYearDB = _db.SEMESTERs.Where(x => x.IdYear == idYear).OrderBy(x => x.DateStart).ToList();
            var listIdSemester = semesterInYearDB.Select(x => x.IdSemester).ToList();

            // Lấy toàn bộ điểm của học sinh
            var allScore = _db.SCOREs.Where(x => x.STUDENT_CLASS.IdStudent == studentDB.IdStudent && (listIdSemester.Any(i => i == x.IdSemester) || x.IdYear == idYear)).ToList();

            // Lấy theo từng môn
            result.Subjects = new List<ScoreSubjectViewModel>();
            foreach (var subject in allSubjectInClass)
            {
                var scoreInfo = new ScoreSubjectViewModel
                {
                    SubjectName = subject.SubjectName,
                    IdSubject = subject.IdSubject,
                    Details = new List<ScoreDetailViewModel>(),
                };

                // Nạp điểm từng học kỳ
                foreach (var semester in semesterInYearDB)
                {
                    var scoreAvgSemester = allScore.Where(x => x.IdScoreType == null && x.IdSubject == subject.IdSubject && x.IdSemester == semester.IdSemester).FirstOrDefault();

                    if (scoreAvgSemester != null)
                    {
                        // Lấy điểm trong học kỳ
                        scoreInfo.Details.Add(new ScoreDetailViewModel
                        {
                            ScoreName = scoreAvgSemester.SEMESTER.SemesterName,
                            Score = scoreAvgSemester?.Score1,
                            IsReadOnly = true,
                        });
                    }
                    else
                    {
                        // Lấy điểm trong học kỳ
                        scoreInfo.Details.Add(new ScoreDetailViewModel
                        {
                            ScoreName = semester.SemesterName,
                            Score = null,
                            IsReadOnly = true,
                        });
                    }
                }

                // Thêm điểm cả năm
                var scoreAvgYear = allScore.Where(x => x.IdScoreType == null && x.IdSubject == subject.IdSubject && x.IdSemester == null).FirstOrDefault();
                if (scoreAvgYear != null)
                {
                    // Lấy điểm trong học kỳ
                    scoreInfo.Details.Add(new ScoreDetailViewModel
                    {
                        ScoreName = "Cả năm",
                        Score = scoreAvgYear?.Score1,
                        IsReadOnly = true,
                    });
                }
                else
                {
                    // Lấy điểm trong học kỳ
                    scoreInfo.Details.Add(new ScoreDetailViewModel
                    {
                        ScoreName = "Cả năm",
                        Score = null,
                        IsReadOnly = true,
                    });
                }

                result.Subjects.Add(scoreInfo);
            }

            // Define list hạnh kiểm
            var listBehaviour = new List<OptionViewModel>();
            listBehaviour.Add(new OptionViewModel
            {
                Text = "",
                Value = "null",
            });
            foreach (BehaviourEnum behaviour in Enum.GetValues(typeof(BehaviourEnum)))
            {
                listBehaviour.Add(new OptionViewModel
                {
                    Text = behaviour.GetAttribute<DisplayAttribute>().Name,
                    Value = ((int)behaviour).ToString(),
                });
            }

            // Lấy thông tin xếp hạng từ db
            var rankDBs = _db.SEMESTERRANKs.Where(x => x.STUDENT_CLASS.STUDENT.IdStudent == studentDB.IdStudent &&
            (listIdSemester.Any(i => i == x.IdSemester) || x.IdYear == idYear)).ToList();

            for (var i = 0; i <= semesterInYearDB.Count; i++)
            {
                SEMESTERRANK rankDB = null;
                ScoreSubjectViewModel semesterData = null;
                if (i != semesterInYearDB.Count)
                {
                    rankDB = rankDBs.Where(x => x.IdSemester == semesterInYearDB[i].IdSemester).FirstOrDefault();
                    semesterData = new ScoreSubjectViewModel
                    {
                        IdYear = semesterInYearDB[i].IdYear ?? -1,
                        IdSemester = semesterInYearDB[i].IdSemester,
                        SubjectName = semesterInYearDB[i].SemesterName,
                        Details = new List<ScoreDetailViewModel>(),
                    };
                }
                else
                {
                    rankDB = rankDBs.Where(x => x.IdYear == idYear && x.IdSemester is null).FirstOrDefault();
                    semesterData = new ScoreSubjectViewModel
                    {
                        IdYear = semesterInYearDB.FirstOrDefault()?.IdYear ?? -1,
                        SubjectName = semesterInYearDB.FirstOrDefault()?.YEAR.YearName,
                        Details = new List<ScoreDetailViewModel>(),
                    };
                }

                // Nạp TBHK
                semesterData.Details.Add(new ScoreDetailViewModel
                {
                    ScoreName = "TB",
                    Score = rankDB?.AvgScore,
                    IsReadOnly = true,
                });

                // Nạp hạnh kiểm
                var behaviourName = "";
                if (rankDB?.IdBehaviour != null)
                {
                    behaviourName = ((BehaviourEnum)rankDB?.IdBehaviour).GetAttribute<DisplayAttribute>().Name;
                }
                semesterData.Details.Add(new ScoreDetailViewModel
                {
                    ScoreName = "HK",
                    Text = behaviourName,
                    SelectedValueOption = rankDB?.IdBehaviour.ToString(),
                    IsOption = true,
                    ListOption = listBehaviour,
                    IsReadOnly = !allowEditBehaviour,
                });

                // Nạp xếp loại
                var rankName = "";
                if (rankDB?.IdRank != null)
                {
                    rankName = ((RankEnum)rankDB?.IdRank).GetAttribute<DisplayAttribute>().Name;
                }

                semesterData.Details.Add(new ScoreDetailViewModel
                {
                    ScoreName = "Xếp loại",
                    Text = rankName,
                    IsReadOnly = true,
                });

                result.Subjects.Add(semesterData);
            }

            return result;
        }

        /// <summary>
        /// Thêm điểm cho học sinh
        /// </summary>
        /// <param name="idAdmin"></param>
        /// <param name="idUser"></param>
        /// <param name="idSubject"></param>
        /// <param name="idSemester"></param>
        /// <param name="scoreType"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public BaseResponse AddStudentScore(int idAdmin, int idUser, int idSubject, int idSemester, int scoreTypeId, double? score, string note = null)
        {
            // Kiểm tra dữ liệu
            if (score.HasValue && (score.Value < 0 || score.Value > 10))
            {
                return new BaseResponse("Điểm số không hợp lệ, phải nằm trong đoạn từ 0 đến 10");
            }

            // Kiểm tra quyền hạn
            if (!IsEditableStudentScore(idAdmin, idUser, idSemester, idSubject))
            {
                return new BaseResponse("Tài khoản không có quyền chỉnh sửa điểm học sinh");
            }

            // Kiểm tra thông tin student
            var studentDB = _db.STUDENTs.Where(x => x.IdUser == idUser).FirstOrDefault();
            if (studentDB is null)
            {
                return new BaseResponse("Học sinh này không tồn tại");
            }

            // Kiểm tra lớp học của học sinh
            var classDB = studentDB.STUDENT_CLASS.Where(x => x.CLASS.YEAR.SEMESTERs.Any(i => i.IdSemester == idSemester)).FirstOrDefault();
            if (classDB is null)
            {
                return new BaseResponse("Dữ liệu lớp học không tồn tại");
            }

            // Kiểm tra semester có nằm trong lớp này hay ko
            if (!classDB.CLASS.YEAR.SEMESTERs.Any(x => x.IdSemester == idSemester))
            {
                return new GetClassScoreResponse
                {
                    IsSuccess = false,
                    Message = "Học kỳ không nằm trong năm học của lớp",
                };
            }

            // Kiểm tra môn học
            if (!classDB.CLASS.CLASS_SUBJECT.Any(x => x.IdSubject == idSubject))
            {
                return new BaseResponse("Dữ liệu lmôn học không tồn tại với lớp này");
            }

            // Tạo log
            string log = string.Empty;

            // Thêm hoặc sửa điểm
            var scoreSubjectDB = classDB.SCOREs.Where(x => x.IdSubject == idSubject && x.IdScoreType == scoreTypeId && x.IdSemester == idSemester).FirstOrDefault();
            var scoreType = _db.SCORE_TYPE.Where(x => x.IdScoreType == scoreTypeId && x.CLASS_SUBJECT.IdClass == classDB.IdClass && x.CLASS_SUBJECT.IdSubject == idSubject).FirstOrDefault();
            if (scoreType is null)
            {
                return new BaseResponse("Không tìm thấy cột điểm mà bạn muốn chỉnh sửa");
            }

            if (scoreSubjectDB is null)
            {
                log = $"Thêm {score} điểm vào cột [{scoreType.ScoreName}]";
                scoreSubjectDB = new SCORE
                {
                    IdSubject = idSubject,
                    IdSemester = idSemester,
                    IdScoreType = scoreTypeId,
                    IdStudentClass = classDB.IdStudentClass,
                    ScoreNote = note,
                };
                _db.SCOREs.Add(scoreSubjectDB);
            }
            else
            {
                log = $"Sửa từ {{0}} điểm thành {{1}} điểm vào cột [{scoreType.ScoreName}]";
            }

            scoreSubjectDB.Score1 = score;
            scoreSubjectDB.ScoreNote = note;

            // Nạp log
            log = log.Replace("{0}", scoreSubjectDB.Score1?.ToString() ?? "[rỗng]");
            log = log.Replace("{1}", score?.ToString() ?? "[rỗng]");

            try
            {
                // Lưu lại
                _db.SaveChanges();

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
                // Lưu log
                AddScoreLog(idAdmin, scoreSubjectDB.IdScore, log);

                // Update điểm TB
                var scoreSemesterService = new ScoreSemesterService();
                scoreSemesterService.CalculateSubjectAvgSingleStudent(idUser, idSemester, classDB.CLASS.IdYear ?? 0);
            }
        }

        /// <summary>
        /// Kiểm tra xem có được phép chỉnh sửa điểm của học sinh hay ko
        /// </summary>
        /// <param name="idAdmin"></param>
        /// <param name="idUser"></param>
        /// <param name="idSemester"></param>
        /// <returns></returns>
        public bool IsEditableStudentScore(int idAdmin, int idUser, int idSemester, int idSubject)
        {
            // Kiểm tra admin
            var adminDB = _db.USERs.Where(x => x.IdUser == idAdmin && x.IsDeleted != true).FirstOrDefault();
            if (adminDB is null)
            {
                return false;
            }

            switch ((RoleEnum)adminDB.IdRole)
            {
                // Nếu admin có role của manager thì được quyền edit
                case RoleEnum.MANAGER:
                case RoleEnum.ASSISTANT:
                    return true;

                // Nếu là giáo viên thì kiểm tra phải giáo viên bộ môn ko
                case RoleEnum.HEAD_OF_SUBJECT:
                case RoleEnum.TEACHER:
                    var classDB = _db.CLASSes.Where(x => (x.YEAR.SEMESTERs.Any(i => i.IdSemester == idSemester)) && x.STUDENT_CLASS.Any(s => s.STUDENT.IdUser == idUser)).FirstOrDefault();
                    if (classDB is null)
                    {
                        return false;
                    }

                    // GVBM đang giảng dạy mới có quyền chỉnh sửa
                    var subjectClass = classDB.CLASS_SUBJECT.Where(x => x.IdSubject == idSubject).FirstOrDefault();
                    if (subjectClass is null)
                    {
                        return false;
                    }

                    if (subjectClass.TEACHER_SUBJECT is null)
                    {
                        return false;
                    }

                    return subjectClass.TEACHER_SUBJECT.IdTeacher == adminDB.TEACHERs.First().IdTeacher;
            }

            // Mặc định ko cho phép
            return false;
        }

        /// <summary>
        /// Lưu log score
        /// </summary>
        /// <param name="idAdmin"></param>
        /// <param name="idScore"></param>
        /// <param name="log"></param>
        private void AddScoreLog(int idAdmin, int idScore, string log)
        {
            var scoreLog = new SCORE_LOG
            {
                IdScore = idScore,
                IdUser = idAdmin,
                CreatedDate = DateTime.Now,
                LogMessage = log,
            };

            _db.SCORE_LOG.Add(scoreLog);
            _db.SaveChanges();
        }

        /// <summary>
        /// Import từ file excel
        /// </summary>
        /// <param name="idAdmin"></param>
        /// <param name="idClass"></param>
        /// <param name="dataFromExcel"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BaseResponse ImportScoreFromExcel(int idAdmin, int idClass, int idSemester, ImportScoreExcel dataFromExcel)
        {
            // Lấy thông tin user
            var adminDB = _db.USERs.AsNoTracking().Where(x => x.IdUser == idAdmin).FirstOrDefault();
            if (adminDB is null)
            {
                return new BaseResponse("Người dùng không tồn tại");
            }

            // Lấy lớp
            var classDB = _db.CLASSes.AsNoTracking().Where(x => x.IdClass == idClass).FirstOrDefault();

            if (classDB is null)
            {
                return new BaseResponse("Lớp học này không tồn tại");
            }

            // Kiểm tra data từ excel
            var subjectDB = classDB.CLASS_SUBJECT.Where(x => x.SUBJECT.SubjectCode.Equals(dataFromExcel.SubjectCode, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (subjectDB is null)
            {
                return new BaseResponse($"Không tìm thấy môn học có mã là '{dataFromExcel.SubjectCode}' trong lớp này");
            }

            // Kiểm tra học sinh trong lớp
            var listStudentCodeDB = classDB.STUDENT_CLASS.Select(x => x.STUDENT.StudentCode).ToList();
            var listStudentCodeExcel = dataFromExcel.Student.Select(x => x.StudentCode).ToList();
            var listDiff = listStudentCodeExcel.Except(listStudentCodeDB);
            if (listDiff.Any())
            {
                return new BaseResponse("Một số học sinh có mã không tồn tại trong lớp: " + string.Join(", ", listDiff));
            }

            // Kiểm tra quyền hạn
            var idSubject = subjectDB.IdSubject;
            var idUserStudent = classDB.STUDENT_CLASS.First().STUDENT.IdUser;
            if (!IsEditableStudentScore(idAdmin, idUserStudent, idSemester, idSubject))
            {
                return new BaseResponse("Bạn không có quyền hạn chỉnh sửa điểm");
            }

            // Upload điểm
            var scoreTypeDB = subjectDB.SCORE_TYPE.ToList();
            var studentList = classDB.STUDENT_CLASS.Select(x => x.STUDENT).ToList();

            var listUpdated = new List<UpdatedScoreResponse>();
            foreach (var excel in dataFromExcel.Student)
            {
                var studentDB = studentList.Where(x => x.StudentCode.Equals(excel.StudentCode, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                // Lấy id score type
                excel.IdScoreType = scoreTypeDB.Where(x => x.ScoreName.Equals(excel.ScoreName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault()?.IdScoreType ?? 0;
                if (excel.IdScoreType == 0) continue;

                AddStudentScore(idAdmin, studentDB.IdUser, idSubject, idSemester, excel.IdScoreType, excel.Score, excel.Note);
                listUpdated.Add(new UpdatedScoreResponse
                {
                    IdScoreType = excel.IdScoreType,
                    IdUser = studentDB.IdUser,
                });

                // Send mail nếu score bị thay đổi trong học kỳ khác học kỳ hiện tại
                CheckBaselineAndSendMail(studentDB.IdUser, idSemester);
            }

            return new BaseResponse
            {
                IsSuccess = true,
                Object = listUpdated,
            };
        }

        public BaseResponse CheckImportScore(int idAdmin, int idClass, int idSemester, ImportScoreExcel dataFromExcel)
        {
            // Lấy lớp
            var classDB = _db.CLASSes.AsNoTracking().Where(x => x.IdClass == idClass).FirstOrDefault();

            if (classDB is null)
            {
                return new BaseResponse("Lớp học này không tồn tại");
            }

            // Kiểm tra data từ excel
            var subjectDB = classDB.CLASS_SUBJECT.Where(x => x.SUBJECT.SubjectCode.Equals(dataFromExcel.SubjectCode, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (subjectDB is null)
            {
                return new BaseResponse($"Không tìm thấy môn học có mã là '{dataFromExcel.SubjectCode}' trong lớp này");
            }

            // Kiểm tra học sinh trong lớp
            var listStudentCodeDB = classDB.STUDENT_CLASS.Select(x => x.STUDENT.StudentCode).ToList();
            var listStudentCodeExcel = dataFromExcel.Student.Select(x => x.StudentCode).ToList();
            var listDiff = listStudentCodeExcel.Except(listStudentCodeDB);
            if (listDiff.Any())
            {
                return new BaseResponse("Một số học sinh có mã không tồn tại trong lớp: " + string.Join(", ", listDiff));
            }

            // Kiểm tra quyền hạn
            var idSubject = subjectDB.IdSubject;
            var idUserStudent = classDB.STUDENT_CLASS.First().STUDENT.IdUser;
            if (!IsEditableStudentScore(idAdmin, idUserStudent, idSemester, idSubject))
            {
                return new BaseResponse("Bạn không có quyền hạn chỉnh sửa điểm");
            }

            // Upload điểm
            var scoreTypeDB = subjectDB.SCORE_TYPE.ToList();
            var studentList = dataFromExcel.Student.Select(x => x.StudentCode).Distinct().ToList();
            var missingMsg = string.Empty;
            foreach (var studentCode in studentList)
            {
                foreach (var type in scoreTypeDB)
                {
                    var scoreExcel = dataFromExcel.Student.Where(x => x.ScoreName.Equals(type.ScoreName, StringComparison.OrdinalIgnoreCase) && x.StudentCode.Equals(studentCode)).FirstOrDefault();
                    if (scoreExcel is null)
                    {
                        missingMsg += $"Học sinh có mã số <b>{studentCode}</b> thiếu cột điểm <b>{type.ScoreName}</b><br>";
                    } else if (scoreExcel.Score < 0 || scoreExcel.Score > 10)
                    {
                        missingMsg += $"Học sinh có mã số <b>{studentCode}</b> có điểm trong cột điểm <b>{type.ScoreName}</b> không hợp lệ<br>";
                    }
                }
            }

            return new BaseResponse
            {
                IsSuccess = true,
                Message = missingMsg,
            };
        }

        /// <summary>
        /// Lấy lịch sử log
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="idSemester"></param>
        /// <returns></returns>
        public BaseResponse GetScoreLog(int idUser, int idSemester)
        {
            // Lấy thông tin học sinh
            var studentDB = _db.STUDENTs.Where(x => x.IdUser == idUser).FirstOrDefault();
            if (studentDB is null)
            {
                return new BaseResponse("Không tìm thấy học sinh này");
            }

            // Lấy toàn bộ điểm hiện có
            var scoresDB = _db.SCOREs.Where(x => x.IdSemester == idSemester && x.STUDENT_CLASS.IdStudent == studentDB.IdStudent).ToList();

            // Duyệt từng score đưa vào list
            var listLog = new List<HistoryScoreViewModel>();
            foreach (var score in scoresDB)
            {
                if (score.SCORE_TYPE is null) continue;

                HistoryScoreViewModel subjectLog = listLog.Where(x => x.IdSubject == score.IdSubject).FirstOrDefault();

                if (subjectLog is null)
                {
                    subjectLog = new HistoryScoreViewModel
                    {
                        IdSubject = score.IdSubject,
                        SubjectName = score.SUBJECT.SubjectName,
                        Logs = new List<HistoryScoreLogViewModel>(),
                    };
                    listLog.Add(subjectLog);
                }

                // Lấy từng log trong môn học
                foreach (var logDB in score.SCORE_LOG.OrderByDescending(x => x.CreatedDate))
                {
                    subjectLog.Logs.Add(new HistoryScoreLogViewModel
                    {
                        DateUpdate = logDB.CreatedDate?.ToString("dd/MM/yyyy lúc HH:mm:ss"),
                        LogDate = logDB.CreatedDate ?? DateTime.Now,
                        UpdateBy = logDB.IdUser,
                        UpdateByName = logDB.USER.Fullname,
                        Log = logDB.LogMessage,
                    });
                }

                subjectLog.Logs = subjectLog.Logs.OrderBy(x => x.LogDate).ToList();
            }

            return new BaseResponse
            {
                IsSuccess = true,
                Object = listLog,
            };
        }

        /// <summary>
        /// Check base line của điểm và send mail nếu điểm bị thay đổi
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="idSemester"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void CheckBaselineAndSendMail(int idUser, int idSemester)
        {
            // Lấy current semester
            int idCurrentSemester = (new SemesterService()).GetCurrentSemesterId() ?? -1;

            // So sánh với hiện tại
            if (idCurrentSemester == idSemester)
            {
                return;
            }

            // Lấy toàn bộ email cần send
            var studentUserDB = _db.USERs.Where(x => x.IdUser == idUser).FirstOrDefault();
            if (studentUserDB is null)
            {
                return;
            }

            // Lấy semester mà điểm số đó thay đổi
            var semesterDB = _db.SEMESTERs.Where(x => x.IdSemester == idSemester).FirstOrDefault();
            if (semesterDB is null)
            {
                return;
            }

            var listEmail = new List<string>();
            if (!string.IsNullOrEmpty(studentUserDB.Email))
            {
                listEmail.Add(studentUserDB.Email);
            }

            // Lấy email của phụ huynh
            var listParentEmail = studentUserDB.STUDENTs
                .First()
                .USERs
                .Where(x => !string.IsNullOrEmpty(x.Email))
                .Select(x => x.Email)
                .ToList();
            listEmail.AddRange(listParentEmail);

            // Gửi mail bằng bg
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
                return;
            }

            var semesterName = semesterDB.SemesterName;
            var semesterYear = semesterDB.YEAR.YearName;
            var studentFullname = studentUserDB.Fullname;
            var emailTemplate = settingService.GetSetting(SettingEnum.EMAIL_TEMPLATE_SCOREBASELINE) ?? String.Empty;
            emailTemplate = emailTemplate.Replace("{fullname}", studentFullname);
            emailTemplate = emailTemplate.Replace("{semester}", semesterName + " - " + semesterYear);

            var emailUtility = new EmailUtility(senderName, emailUsername, emailPassword, smtp, port);

            try
            {
                Thread trd = new Thread(new ThreadStart(() =>
                {
                    emailUtility.Send(listEmail, "ĐIỂM SỐ ĐƯỢC THAY ĐỔI", emailTemplate);
                }));
                trd.IsBackground = true;
                trd.Start();
            }
            catch
            {
                // Do nothing
            }
        }
    }
}

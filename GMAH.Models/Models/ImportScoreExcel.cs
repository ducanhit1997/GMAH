using System.Collections.Generic;

namespace GMAH.Models.Models
{
    public class ImportScoreExcel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string SubjectCode { get; set; }
        public List<ImportScoreStudentDetail> Student { get; set; }
    }

    public class ImportScoreStudentDetail
    {
        public string StudentCode { get; set; }
        public string StudentName { get; set; }
        public double? Score { get; set; }
        public int IdScoreType { get; set; }
        public string ScoreName { get; set; }
        public string Note { get; set; }
    }
}

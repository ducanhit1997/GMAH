namespace GMAH.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SEMESTERRANK")]
    public partial class SEMESTERRANK
    {
        [Key]
        public int IdSemesterRank { get; set; }

        public int? IdSemester { get; set; }

        public int? IdYear { get; set; }

        public int IdStudentClass { get; set; }

        public double? AvgScore { get; set; }

        public int? IdRank { get; set; }

        public int? IdBehaviour { get; set; }

        public virtual SEMESTER SEMESTER { get; set; }

        public virtual STUDENT_CLASS STUDENT_CLASS { get; set; }

        public virtual YEAR YEAR { get; set; }
    }
}

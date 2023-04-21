namespace GMAH.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SCORE")]
    public partial class SCORE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SCORE()
        {
            SCORE_LOG = new HashSet<SCORE_LOG>();
        }

        [Key]
        public int IdScore { get; set; }

        public int IdSubject { get; set; }

        public int IdStudentClass { get; set; }

        [Column("Score")]
        public double? Score1 { get; set; }

        public int? IdSemester { get; set; }

        public int? IdScoreType { get; set; }

        [StringLength(500)]
        public string ScoreNote { get; set; }

        public int? IdYear { get; set; }

        public virtual SEMESTER SEMESTER { get; set; }

        public virtual STUDENT_CLASS STUDENT_CLASS { get; set; }

        public virtual SUBJECT SUBJECT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SCORE_LOG> SCORE_LOG { get; set; }

        public virtual SCORE_TYPE SCORE_TYPE { get; set; }

        public virtual YEAR YEAR { get; set; }
    }
}

namespace GMAH.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SEMESTER")]
    public partial class SEMESTER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SEMESTER()
        {
            GRADERULEs = new HashSet<GRADERULE>();
            SCOREs = new HashSet<SCORE>();
            SEMESTERRANKs = new HashSet<SEMESTERRANK>();
            TIMELINEs = new HashSet<TIMELINE>();
        }

        [Key]
        public int IdSemester { get; set; }

        [StringLength(50)]
        public string SemesterName { get; set; }

        public bool? IsCurrentSemester { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DateStart { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? DateEnd { get; set; }

        public int? ScoreWeight { get; set; }

        public int? IdYear { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GRADERULE> GRADERULEs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SCORE> SCOREs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SEMESTERRANK> SEMESTERRANKs { get; set; }
        public virtual ICollection<TIMELINE> TIMELINEs { get; set; }

        public virtual YEAR YEAR { get; set; }
    }
}

namespace GMAH.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CLASS")]
    public partial class CLASS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CLASS()
        {
            CLASS_SUBJECT = new HashSet<CLASS_SUBJECT>();
            STUDENT_CLASS = new HashSet<STUDENT_CLASS>();
            TIMELINEs = new HashSet<TIMELINE>();
        }

        [Key]
        public int IdClass { get; set; }

        [StringLength(50)]
        public string ClassName { get; set; }

        public int? IdFormTeacher { get; set; }

        public int? IdRule { get; set; }

        public int? IdField { get; set; }

        public int? IdYear { get; set; }

        public virtual FIELDSTUDY FIELDSTUDY { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLASS_SUBJECT> CLASS_SUBJECT { get; set; }

        public virtual YEAR YEAR { get; set; }

        public virtual GRADERULE GRADERULE { get; set; }

        public virtual TEACHER TEACHER { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_CLASS> STUDENT_CLASS { get; set; }
        public virtual ICollection<TIMELINE> TIMELINEs { get; set; }

    }
}

namespace GMAH.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SUBJECT")]
    public partial class SUBJECT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SUBJECT()
        {
            CLASS_SUBJECT = new HashSet<CLASS_SUBJECT>();
            GRADERULEDETAILs = new HashSet<GRADERULEDETAIL>();
            HEAD_OF_SUBJECT = new HashSet<HEAD_OF_SUBJECT>();
            SCOREs = new HashSet<SCORE>();
            TEACHER_SUBJECT = new HashSet<TEACHER_SUBJECT>();
            TIMELINEs = new HashSet<TIMELINE>();
        }

        [Key]
        public int IdSubject { get; set; }

        [StringLength(50)]
        public string SubjectName { get; set; }

        [StringLength(20)]
        public string SubjectCode { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLASS_SUBJECT> CLASS_SUBJECT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GRADERULEDETAIL> GRADERULEDETAILs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HEAD_OF_SUBJECT> HEAD_OF_SUBJECT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SCORE> SCOREs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEACHER_SUBJECT> TEACHER_SUBJECT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIMELINE> TIMELINEs { get; set; }
    }
}

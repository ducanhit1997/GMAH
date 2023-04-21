namespace GMAH.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CLASS_SUBJECT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CLASS_SUBJECT()
        {
            SCORE_TYPE = new HashSet<SCORE_TYPE>();
        }

        [Key]
        public int IdClassSubject { get; set; }

        public int IdClass { get; set; }

        public int? IdTeacherSubject { get; set; }

        public int IdSubject { get; set; }

        public virtual CLASS CLASS { get; set; }

        public virtual SUBJECT SUBJECT { get; set; }

        public virtual TEACHER_SUBJECT TEACHER_SUBJECT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SCORE_TYPE> SCORE_TYPE { get; set; }
    }
}

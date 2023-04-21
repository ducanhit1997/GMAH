namespace GMAH.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TEACHER_SUBJECT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TEACHER_SUBJECT()
        {
            CLASS_SUBJECT = new HashSet<CLASS_SUBJECT>();
        }

        [Key]
        public int IdTeacherSubject { get; set; }

        public int IdTeacher { get; set; }

        public int IdSubject { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLASS_SUBJECT> CLASS_SUBJECT { get; set; }

        public virtual SUBJECT SUBJECT { get; set; }

        public virtual TEACHER TEACHER { get; set; }
    }
}

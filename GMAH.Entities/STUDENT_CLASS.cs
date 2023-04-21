namespace GMAH.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class STUDENT_CLASS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public STUDENT_CLASS()
        {
            ATTENDANCEs = new HashSet<ATTENDANCE>();
            SCOREs = new HashSet<SCORE>();
            SEMESTERRANKs = new HashSet<SEMESTERRANK>();
        }

        [Key]
        public int IdStudentClass { get; set; }

        public int IdStudent { get; set; }

        public int IdClass { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTENDANCE> ATTENDANCEs { get; set; }

        public virtual CLASS CLASS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SCORE> SCOREs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SEMESTERRANK> SEMESTERRANKs { get; set; }

        public virtual STUDENT STUDENT { get; set; }
    }
}

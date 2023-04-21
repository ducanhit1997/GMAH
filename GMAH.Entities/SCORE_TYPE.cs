namespace GMAH.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SCORE_TYPE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SCORE_TYPE()
        {
            SCOREs = new HashSet<SCORE>();
        }

        [Key]
        public int IdScoreType { get; set; }

        [StringLength(50)]
        public string ScoreName { get; set; }

        public byte? ScoreWeight { get; set; }

        public int? IdClassSubject { get; set; }

        public virtual CLASS_SUBJECT CLASS_SUBJECT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SCORE> SCOREs { get; set; }
    }
}

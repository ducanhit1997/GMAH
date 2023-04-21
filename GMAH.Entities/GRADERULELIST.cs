namespace GMAH.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GRADERULELIST")]
    public partial class GRADERULELIST
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GRADERULELIST()
        {
            GRADERULEDETAILs = new HashSet<GRADERULEDETAIL>();
        }

        [Key]
        public int IdRuleList { get; set; }

        public int IdRule { get; set; }

        public int IdRank { get; set; }

        public double? MinAvgScore { get; set; }

        public int? IdBehaviour { get; set; }

        public virtual GRADERULE GRADERULE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GRADERULEDETAIL> GRADERULEDETAILs { get; set; }
    }
}

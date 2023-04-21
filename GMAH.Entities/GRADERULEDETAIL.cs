namespace GMAH.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GRADERULEDETAIL")]
    public partial class GRADERULEDETAIL
    {
        [Key]
        public int IdRuleDetail { get; set; }

        public int IdRuleList { get; set; }

        public int? IdSubject { get; set; }

        public float MinAvgScore { get; set; }

        public virtual GRADERULELIST GRADERULELIST { get; set; }

        public virtual SUBJECT SUBJECT { get; set; }
    }
}

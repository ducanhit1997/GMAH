namespace GMAH.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TIMELINE")]
    public partial class TIMELINE
    {
        [Key]
        public int IdSchedule { get; set; }

        public int IdSubject { get; set; }
        public int IdClass { get; set; }
        public int IdSemester { get; set; }

        public int? Period { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }

        public virtual SUBJECT SUBJECT { get; set; }
        public virtual CLASS CLASS { get; set; }
        public virtual SEMESTER SEMESTER { get; set; }

    }
}

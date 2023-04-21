namespace GMAH.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class HEAD_OF_SUBJECT
    {
        [Key]
        public int IdHeadOfSubject { get; set; }

        public int IdTeacher { get; set; }

        public int IdSubject { get; set; }

        public int? FromYear { get; set; }

        public int? ToYear { get; set; }

        public virtual SUBJECT SUBJECT { get; set; }

        public virtual TEACHER TEACHER { get; set; }
    }
}

namespace GMAH.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SCORE_LOG
    {
        [Key]
        public int IdScoreLog { get; set; }

        public int IdUser { get; set; }

        public int IdScore { get; set; }

        [Column(TypeName = "ntext")]
        public string LogMessage { get; set; }

        public DateTime? CreatedDate { get; set; }

        public virtual SCORE SCORE { get; set; }

        public virtual USER USER { get; set; }
    }
}

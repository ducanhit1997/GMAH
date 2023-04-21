namespace GMAH.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class REPORT_HISTORY
    {
        [Key]
        public int IdReportHistory { get; set; }

        public DateTime? HistoryDate { get; set; }

        public int IdUserUpdate { get; set; }

        public int ReportStatus { get; set; }

        [Column(TypeName = "ntext")]
        public string Comment { get; set; }

        public int IdReport { get; set; }

        public virtual REPORT REPORT { get; set; }

        public virtual REPORT_STATUS REPORT_STATUS { get; set; }

        public virtual USER USER { get; set; }
    }
}

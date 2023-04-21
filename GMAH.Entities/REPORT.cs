namespace GMAH.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("REPORT")]
    public partial class REPORT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public REPORT()
        {
            REPORT_HISTORY = new HashSet<REPORT_HISTORY>();
            REPORT_FILE = new HashSet<REPORT_FILE>();
        }

        [Key]
        public int IdReport { get; set; }

        public int? ReportType { get; set; }

        [StringLength(255)]
        public string ReportTitle { get; set; }

        [Column(TypeName = "ntext")]
        public string ReportContent { get; set; }

        public int ReportStatus { get; set; }

        public int IdUserSubmitReport { get; set; }

        public int SubmitForIdUser { get; set; }


        public DateTime? SubmitDate { get; set; }

        public DateTime? LastUpdateDate { get; set; }

        [StringLength(500)]
        public string EditField { get; set; }

        [StringLength(500)]
        public string EditValue { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REPORT_HISTORY> REPORT_HISTORY { get; set; }
        public virtual ICollection<REPORT_FILE> REPORT_FILE { get; set; }

        public virtual REPORT_STATUS REPORT_STATUS { get; set; }

        public virtual USER USER { get; set; }
        public virtual USER STUDENTUSER { get; set; }

    }
}

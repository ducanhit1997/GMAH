using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace GMAH.Entities
{
    public partial class REPORT_FILE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public REPORT_FILE()
        {
        }

        [Key]
        public int IdReportFile { get; set; }
        public int IdReport { get; set; }

        [StringLength(500)]
        public string Filename { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual REPORT REPORT { get; set; }
    }
}

namespace GMAH.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ATTENDANCE")]
    public partial class ATTENDANCE
    {
        [Key]
        public int IdAttendance { get; set; }

        public int IdStudentClass { get; set; }

        public DateTime? CheckinTime { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateAttendance { get; set; }

        public bool? IsAvailable { get; set; }

        public bool? IsLeavePermission { get; set; }

        public int AssistantID { get; set; }

        public virtual USER USER { get; set; }

        public virtual STUDENT_CLASS STUDENT_CLASS { get; set; }
    }
}

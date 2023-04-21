namespace GMAH.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("USER")]
    public partial class USER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public USER()
        {
            ATTENDANCEs = new HashSet<ATTENDANCE>();
            REPORTs = new HashSet<REPORT>();
            REPORT_HISTORY = new HashSet<REPORT_HISTORY>();
            SCORE_LOG = new HashSet<SCORE_LOG>();
            STUDENTs = new HashSet<STUDENT>();
            TEACHERs = new HashSet<TEACHER>();
            STUDENTs1 = new HashSet<STUDENT>();
            MYREPORTs = new HashSet<REPORT>();
        }

        [Key]
        public int IdUser { get; set; }

        public int IdRole { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [StringLength(255)]
        public string HashPassword { get; set; }


        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(50)]
        public string Fullname { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(10)]
        public string Phone { get; set; }

        public bool? IsDeleted { get; set; }

        [StringLength(50)]
        public string CitizenId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTENDANCE> ATTENDANCEs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REPORT> REPORTs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REPORT_HISTORY> REPORT_HISTORY { get; set; }

        public virtual ROLE ROLE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SCORE_LOG> SCORE_LOG { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT> STUDENTs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEACHER> TEACHERs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT> STUDENTs1 { get; set; }
        public virtual ICollection<REPORT> MYREPORTs { get; set; }

    }
}

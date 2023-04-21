namespace GMAH.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SYSTEMSETTING")]
    public partial class SYSTEMSETTING
    {
        [Key]
        [StringLength(100)]
        public string SettingKey { get; set; }

        [Column(TypeName = "ntext")]
        public string SettingValue { get; set; }

        [StringLength(50)]
        public string InputType { get; set; }

        [StringLength(255)]
        public string SettingName { get; set; }
    }
}

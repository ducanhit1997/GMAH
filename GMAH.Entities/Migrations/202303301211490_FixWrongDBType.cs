namespace GMAH.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixWrongDBType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.REPORT_STATUS", "StatusName", c => c.String(maxLength: 100, fixedLength: true));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.REPORT_STATUS", "StatusName", c => c.String(maxLength: 10, fixedLength: true));
        }
    }
}

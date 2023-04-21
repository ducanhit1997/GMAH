namespace GMAH.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndentityKeyToTimelineTable : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TIMELINE");
            AlterColumn("dbo.TIMELINE", "IdSchedule", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.TIMELINE", "IdSchedule");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.TIMELINE");
            AlterColumn("dbo.TIMELINE", "IdSchedule", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.TIMELINE", "IdSchedule");
        }
    }
}

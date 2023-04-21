namespace GMAH.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyTimeLineTable : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TIMELINE");
            AddColumn("dbo.TIMELINE", "IdClass", c => c.Int(nullable: false));
            AddColumn("dbo.TIMELINE", "IdSemester", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.TIMELINE", "IdSchedule");
            CreateIndex("dbo.TIMELINE", "IdClass");
            CreateIndex("dbo.TIMELINE", "IdSemester");
            AddForeignKey("dbo.TIMELINE", "IdSemester", "dbo.SEMESTER", "IdSemester");
            AddForeignKey("dbo.TIMELINE", "IdClass", "dbo.CLASS", "IdClass");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TIMELINE", "IdClass", "dbo.CLASS");
            DropForeignKey("dbo.TIMELINE", "IdSemester", "dbo.SEMESTER");
            DropIndex("dbo.TIMELINE", new[] { "IdSemester" });
            DropIndex("dbo.TIMELINE", new[] { "IdClass" });
            DropPrimaryKey("dbo.TIMELINE");
            DropColumn("dbo.TIMELINE", "IdSemester");
            DropColumn("dbo.TIMELINE", "IdClass");
            AddPrimaryKey("dbo.TIMELINE", new[] { "IdSchedule", "IdSubject" });
        }
    }
}

namespace GMAH.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUploadReportFile : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.REPORT_FILE",
                c => new
                    {
                        IdReportFile = c.Int(nullable: false, identity: true),
                        IdReport = c.Int(nullable: false),
                        Filename = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.IdReportFile)
                .ForeignKey("dbo.REPORT", t => t.IdReport)
                .Index(t => t.IdReport);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.REPORT_FILE", "IdReport", "dbo.REPORT");
            DropIndex("dbo.REPORT_FILE", new[] { "IdReport" });
            DropTable("dbo.REPORT_FILE");
        }
    }
}

namespace GMAH.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSubmitReportForIdUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.REPORT", "SubmitForIdUser", c => c.Int(nullable: false));
            CreateIndex("dbo.REPORT", "SubmitForIdUser");
            AddForeignKey("dbo.REPORT", "SubmitForIdUser", "dbo.USER", "IdUser");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.REPORT", "SubmitForIdUser", "dbo.USER");
            DropIndex("dbo.REPORT", new[] { "SubmitForIdUser" });
            DropColumn("dbo.REPORT", "SubmitForIdUser");
        }
    }
}

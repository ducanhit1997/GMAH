namespace GMAH.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.USER", "Address", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.USER", "Address");
        }
    }
}

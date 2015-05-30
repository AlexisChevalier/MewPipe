namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedCategoryDefault : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "IsDefault", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categories", "IsDefault");
        }
    }
}

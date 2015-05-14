namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedVideos2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Videos", "Seconds", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Videos", "Seconds");
        }
    }
}

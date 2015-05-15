namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedVideos1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Videos", "DateTimeUtc", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Videos", "DateTimeUtc");
        }
    }
}

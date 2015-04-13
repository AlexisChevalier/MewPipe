namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedVideosPrivacy : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Videos", "PrivacyType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Videos", "PrivacyType");
        }
    }
}

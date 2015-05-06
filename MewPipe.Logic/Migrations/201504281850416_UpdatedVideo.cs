namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedVideo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Videos", "UploadRedirectUri", c => c.String());
            AddColumn("dbo.Videos", "NotificationHookUri", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Videos", "NotificationHookUri");
            DropColumn("dbo.Videos", "UploadRedirectUri");
        }
    }
}

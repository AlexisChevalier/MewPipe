namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedOauthClient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OauthClients", "DialogDisabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OauthClients", "DialogDisabled");
        }
    }
}

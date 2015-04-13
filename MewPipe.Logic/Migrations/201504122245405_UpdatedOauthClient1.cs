namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedOauthClient1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OauthClients", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OauthClients", "Name");
        }
    }
}

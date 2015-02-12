namespace MewPipe.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedScopeToAccessToken : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OauthAccessTokens", "Scope", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OauthAccessTokens", "Scope");
        }
    }
}

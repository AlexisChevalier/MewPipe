using System.Data.Entity.Migrations;

namespace MewPipe.Logic.Migrations
{
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

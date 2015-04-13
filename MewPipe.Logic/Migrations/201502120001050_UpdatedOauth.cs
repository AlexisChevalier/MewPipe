using System.Data.Entity.Migrations;

namespace MewPipe.Logic.Migrations
{
    public partial class UpdatedOauth : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OauthRefreshTokens", "AccessToken_Id", "dbo.OauthAccessTokens");
            DropIndex("dbo.OauthRefreshTokens", new[] { "AccessToken_Id" });
            AddColumn("dbo.OauthRefreshTokens", "Scope", c => c.String());
            AddColumn("dbo.OauthRefreshTokens", "OauthClient_Id", c => c.Int());
            AddColumn("dbo.OauthRefreshTokens", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.OauthRefreshTokens", "OauthClient_Id");
            CreateIndex("dbo.OauthRefreshTokens", "User_Id");
            AddForeignKey("dbo.OauthRefreshTokens", "OauthClient_Id", "dbo.OauthClients", "Id");
            AddForeignKey("dbo.OauthRefreshTokens", "User_Id", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.OauthRefreshTokens", "AccessToken_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OauthRefreshTokens", "AccessToken_Id", c => c.Int());
            DropForeignKey("dbo.OauthRefreshTokens", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.OauthRefreshTokens", "OauthClient_Id", "dbo.OauthClients");
            DropIndex("dbo.OauthRefreshTokens", new[] { "User_Id" });
            DropIndex("dbo.OauthRefreshTokens", new[] { "OauthClient_Id" });
            DropColumn("dbo.OauthRefreshTokens", "User_Id");
            DropColumn("dbo.OauthRefreshTokens", "OauthClient_Id");
            DropColumn("dbo.OauthRefreshTokens", "Scope");
            CreateIndex("dbo.OauthRefreshTokens", "AccessToken_Id");
            AddForeignKey("dbo.OauthRefreshTokens", "AccessToken_Id", "dbo.OauthAccessTokens", "Id");
        }
    }
}

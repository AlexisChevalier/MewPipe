namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OauthAccessTokens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        ExpirationTime = c.DateTime(nullable: false),
                        Token = c.String(),
                        Scope = c.String(),
                        OauthClient_Id = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OauthClients", t => t.OauthClient_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.OauthClient_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.OauthClients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.String(),
                        ClientSecret = c.String(),
                        RedirectUri = c.String(),
                        Description = c.String(),
                        Name = c.String(),
                        ImageSrc = c.String(),
                        DialogDisabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.OauthUserTrusts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Scope = c.String(),
                        OauthClient_Id = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OauthClients", t => t.OauthClient_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.OauthClient_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Videos",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        PublicId = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                        PrivacyStatus = c.Int(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.VideoFiles",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        MimeType_Value = c.String(),
                        Quality_Value = c.String(),
                        ContentLengthInBytes = c.Int(nullable: false),
                        IsOriginalFile = c.Boolean(nullable: false),
                        Video_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Videos", t => t.Video_Id)
                .Index(t => t.Video_Id);
            
            CreateTable(
                "dbo.VideoUploadTokens",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ExpirationTime = c.DateTime(nullable: false),
                        UploadRedirectUri = c.String(),
                        NotificationHookUri = c.String(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.OauthAuthorizationCodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExpirationTime = c.DateTime(nullable: false),
                        Code = c.String(),
                        State = c.String(),
                        Scope = c.String(),
                        RedirectUri = c.String(),
                        OauthClient_Id = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OauthClients", t => t.OauthClient_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.OauthClient_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.OauthRefreshTokens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExpirationTime = c.DateTime(nullable: false),
                        Token = c.String(),
                        Scope = c.String(),
                        OauthClient_Id = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OauthClients", t => t.OauthClient_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.OauthClient_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.UserVideos",
                c => new
                    {
                        User_Id = c.String(nullable: false, maxLength: 128),
                        Video_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Video_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("dbo.Videos", t => t.Video_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.Video_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.OauthRefreshTokens", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.OauthRefreshTokens", "OauthClient_Id", "dbo.OauthClients");
            DropForeignKey("dbo.OauthAuthorizationCodes", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.OauthAuthorizationCodes", "OauthClient_Id", "dbo.OauthClients");
            DropForeignKey("dbo.OauthAccessTokens", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.VideoUploadTokens", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserVideos", "Video_Id", "dbo.Videos");
            DropForeignKey("dbo.UserVideos", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Videos", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.VideoFiles", "Video_Id", "dbo.Videos");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OauthUserTrusts", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.OauthUserTrusts", "OauthClient_Id", "dbo.OauthClients");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OauthAccessTokens", "OauthClient_Id", "dbo.OauthClients");
            DropIndex("dbo.UserVideos", new[] { "Video_Id" });
            DropIndex("dbo.UserVideos", new[] { "User_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.OauthRefreshTokens", new[] { "User_Id" });
            DropIndex("dbo.OauthRefreshTokens", new[] { "OauthClient_Id" });
            DropIndex("dbo.OauthAuthorizationCodes", new[] { "User_Id" });
            DropIndex("dbo.OauthAuthorizationCodes", new[] { "OauthClient_Id" });
            DropIndex("dbo.VideoUploadTokens", new[] { "User_Id" });
            DropIndex("dbo.VideoFiles", new[] { "Video_Id" });
            DropIndex("dbo.Videos", new[] { "User_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.OauthUserTrusts", new[] { "User_Id" });
            DropIndex("dbo.OauthUserTrusts", new[] { "OauthClient_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.OauthAccessTokens", new[] { "User_Id" });
            DropIndex("dbo.OauthAccessTokens", new[] { "OauthClient_Id" });
            DropTable("dbo.UserVideos");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.OauthRefreshTokens");
            DropTable("dbo.OauthAuthorizationCodes");
            DropTable("dbo.VideoUploadTokens");
            DropTable("dbo.VideoFiles");
            DropTable("dbo.Videos");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.OauthUserTrusts");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.OauthClients");
            DropTable("dbo.OauthAccessTokens");
        }
    }
}

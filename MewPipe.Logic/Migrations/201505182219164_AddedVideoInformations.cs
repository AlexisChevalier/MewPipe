namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedVideoInformations : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.UserVideos", newName: "VideoUsers");
            DropForeignKey("dbo.VideoUploadTokens", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.VideoUploadTokens", new[] { "User_Id" });
            DropPrimaryKey("dbo.VideoUsers");
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Impressions",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                        Video_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("dbo.Videos", t => t.Video_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.Video_Id);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TagVideos",
                c => new
                    {
                        Tag_Id = c.Guid(nullable: false),
                        Video_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Video_Id })
                .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)
                .ForeignKey("dbo.Videos", t => t.Video_Id, cascadeDelete: true)
                .Index(t => t.Tag_Id)
                .Index(t => t.Video_Id);
            
            AddColumn("dbo.Videos", "Views", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Videos", "Category_Id", c => c.Guid());
            AlterColumn("dbo.VideoUploadTokens", "User_Id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.VideoUsers", new[] { "Video_Id", "User_Id" });
            CreateIndex("dbo.Videos", "Category_Id");
            CreateIndex("dbo.VideoUploadTokens", "User_Id");
            AddForeignKey("dbo.Videos", "Category_Id", "dbo.Categories", "Id");
            AddForeignKey("dbo.VideoUploadTokens", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VideoUploadTokens", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Videos", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.TagVideos", "Video_Id", "dbo.Videos");
            DropForeignKey("dbo.TagVideos", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.Impressions", "Video_Id", "dbo.Videos");
            DropForeignKey("dbo.Impressions", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.TagVideos", new[] { "Video_Id" });
            DropIndex("dbo.TagVideos", new[] { "Tag_Id" });
            DropIndex("dbo.VideoUploadTokens", new[] { "User_Id" });
            DropIndex("dbo.Impressions", new[] { "Video_Id" });
            DropIndex("dbo.Impressions", new[] { "User_Id" });
            DropIndex("dbo.Videos", new[] { "Category_Id" });
            DropPrimaryKey("dbo.VideoUsers");
            AlterColumn("dbo.VideoUploadTokens", "User_Id", c => c.String(maxLength: 128));
            DropColumn("dbo.Videos", "Category_Id");
            DropColumn("dbo.Videos", "Views");
            DropTable("dbo.TagVideos");
            DropTable("dbo.Tags");
            DropTable("dbo.Impressions");
            DropTable("dbo.Categories");
            AddPrimaryKey("dbo.VideoUsers", new[] { "User_Id", "Video_Id" });
            CreateIndex("dbo.VideoUploadTokens", "User_Id");
            AddForeignKey("dbo.VideoUploadTokens", "User_Id", "dbo.AspNetUsers", "Id");
            RenameTable(name: "dbo.VideoUsers", newName: "UserVideos");
        }
    }
}

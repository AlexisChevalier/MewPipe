namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedVideosRelations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserVideos",
                c => new
                    {
                        User_Id = c.String(nullable: false, maxLength: 128),
                        Video_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Video_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("dbo.Videos", t => t.Video_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.Video_Id);
            
            AddColumn("dbo.Videos", "MimeContentType", c => c.String());
            DropColumn("dbo.Videos", "ContentType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Videos", "ContentType", c => c.String());
            DropForeignKey("dbo.UserVideos", "Video_Id", "dbo.Videos");
            DropForeignKey("dbo.UserVideos", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.UserVideos", new[] { "Video_Id" });
            DropIndex("dbo.UserVideos", new[] { "User_Id" });
            DropColumn("dbo.Videos", "MimeContentType");
            DropTable("dbo.UserVideos");
        }
    }
}

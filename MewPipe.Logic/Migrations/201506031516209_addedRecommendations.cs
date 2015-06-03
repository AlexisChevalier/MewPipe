namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedRecommendations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Recommendations",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Score = c.Double(nullable: false),
                        Video_Id = c.Guid(),
                        Video_Id1 = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Videos", t => t.Video_Id)
                .ForeignKey("dbo.Videos", t => t.Video_Id1, cascadeDelete: true)
                .Index(t => t.Video_Id)
                .Index(t => t.Video_Id1);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Recommendations", "Video_Id1", "dbo.Videos");
            DropForeignKey("dbo.Recommendations", "Video_Id", "dbo.Videos");
            DropIndex("dbo.Recommendations", new[] { "Video_Id1" });
            DropIndex("dbo.Recommendations", new[] { "Video_Id" });
            DropTable("dbo.Recommendations");
        }
    }
}

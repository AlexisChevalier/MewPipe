namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedVideos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Videos",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ContentType = c.String(),
                        ContentLengthInBytes = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Videos", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Videos", new[] { "User_Id" });
            DropTable("dbo.Videos");
        }
    }
}

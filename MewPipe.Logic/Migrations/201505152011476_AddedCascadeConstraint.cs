namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCascadeConstraint : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VideoFiles", "Video_Id", "dbo.Videos");
            AddColumn("dbo.VideoFiles", "Video_Id1", c => c.Guid());
            CreateIndex("dbo.VideoFiles", "Video_Id1");
            AddForeignKey("dbo.VideoFiles", "Video_Id1", "dbo.Videos", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VideoFiles", "Video_Id1", "dbo.Videos");
            DropIndex("dbo.VideoFiles", new[] { "Video_Id1" });
            DropColumn("dbo.VideoFiles", "Video_Id1");
            AddForeignKey("dbo.VideoFiles", "Video_Id", "dbo.Videos", "Id");
        }
    }
}

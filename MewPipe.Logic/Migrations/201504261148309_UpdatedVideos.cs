namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedVideos : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Videos", new[] { "PublicId" });
            DropColumn("dbo.Videos", "PublicId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Videos", "PublicId", c => c.String(maxLength: 40));
            CreateIndex("dbo.Videos", "PublicId", unique: true);
        }
    }
}

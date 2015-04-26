namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedSizeFromVideoFile : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.VideoFiles", "ContentLengthInBytes");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VideoFiles", "ContentLengthInBytes", c => c.Int(nullable: false));
        }
    }
}

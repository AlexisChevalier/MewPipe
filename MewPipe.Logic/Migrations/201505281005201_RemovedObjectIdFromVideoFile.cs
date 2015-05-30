namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedObjectIdFromVideoFile : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.VideoFiles", "GridFsId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VideoFiles", "GridFsId", c => c.String());
        }
    }
}

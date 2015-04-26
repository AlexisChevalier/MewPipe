namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedVideoFile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VideoFiles", "GridFsId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VideoFiles", "GridFsId");
        }
    }
}

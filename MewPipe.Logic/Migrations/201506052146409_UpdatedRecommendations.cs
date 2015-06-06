namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedRecommendations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Impressions", "DateTimeUtc", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Impressions", "DateTimeUtc");
        }
    }
}

namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedQualityType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VideoFiles", "QualityType_Id", c => c.Guid());
            AddColumn("dbo.QualityTypes", "IsUploaded", c => c.Boolean(nullable: false));
            CreateIndex("dbo.VideoFiles", "QualityType_Id");
            AddForeignKey("dbo.VideoFiles", "QualityType_Id", "dbo.QualityTypes", "Id");
            DropColumn("dbo.VideoFiles", "Quality_Value");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VideoFiles", "Quality_Value", c => c.String());
            DropForeignKey("dbo.VideoFiles", "QualityType_Id", "dbo.QualityTypes");
            DropIndex("dbo.VideoFiles", new[] { "QualityType_Id" });
            DropColumn("dbo.QualityTypes", "IsUploaded");
            DropColumn("dbo.VideoFiles", "QualityType_Id");
        }
    }
}

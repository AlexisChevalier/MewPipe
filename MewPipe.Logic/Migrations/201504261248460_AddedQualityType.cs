namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedQualityType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QualityTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(maxLength: 40),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.QualityTypes", new[] { "Name" });
            DropTable("dbo.QualityTypes");
        }
    }
}

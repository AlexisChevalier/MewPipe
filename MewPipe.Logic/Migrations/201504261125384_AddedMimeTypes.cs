namespace MewPipe.Logic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMimeTypes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MimeTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(maxLength: 40),
                        HttpMimeType = c.String(maxLength: 40),
                        AllowedForDecoding = c.Boolean(nullable: false),
                        RequiredForEncoding = c.Boolean(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true)
                .Index(t => t.HttpMimeType, unique: true);
            
            AddColumn("dbo.VideoFiles", "MimeType_Id", c => c.Guid());
            AlterColumn("dbo.Videos", "PublicId", c => c.String(maxLength: 40));
            CreateIndex("dbo.Videos", "PublicId", unique: true);
            CreateIndex("dbo.VideoFiles", "MimeType_Id");
            AddForeignKey("dbo.VideoFiles", "MimeType_Id", "dbo.MimeTypes", "Id");
            DropColumn("dbo.VideoFiles", "MimeType_Value");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VideoFiles", "MimeType_Value", c => c.String());
            DropForeignKey("dbo.VideoFiles", "MimeType_Id", "dbo.MimeTypes");
            DropIndex("dbo.VideoFiles", new[] { "MimeType_Id" });
            DropIndex("dbo.Videos", new[] { "PublicId" });
            DropIndex("dbo.MimeTypes", new[] { "HttpMimeType" });
            DropIndex("dbo.MimeTypes", new[] { "Name" });
            AlterColumn("dbo.Videos", "PublicId", c => c.String());
            DropColumn("dbo.VideoFiles", "MimeType_Id");
            DropTable("dbo.MimeTypes");
        }
    }
}

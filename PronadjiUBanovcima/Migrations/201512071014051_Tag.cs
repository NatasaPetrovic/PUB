namespace PronadjiUBanovcima.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tag : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Naziv = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TagInfoes",
                c => new
                    {
                        Tag_Id = c.Int(nullable: false),
                        Info_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Info_Id })
                .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)
                .ForeignKey("dbo.Infoes", t => t.Info_Id, cascadeDelete: true)
                .Index(t => t.Tag_Id)
                .Index(t => t.Info_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TagInfoes", "Info_Id", "dbo.Infoes");
            DropForeignKey("dbo.TagInfoes", "Tag_Id", "dbo.Tags");
            DropIndex("dbo.TagInfoes", new[] { "Info_Id" });
            DropIndex("dbo.TagInfoes", new[] { "Tag_Id" });
            DropTable("dbo.TagInfoes");
            DropTable("dbo.Tags");
        }
    }
}

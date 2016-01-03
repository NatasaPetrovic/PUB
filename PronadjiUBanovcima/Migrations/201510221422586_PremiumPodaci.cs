namespace PronadjiUBanovcima.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PremiumPodaci : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PremiumInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Opis = c.String(),
                        Sajt = c.String(),
                        Klijent_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Klijent_Id)
                .Index(t => t.Klijent_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PremiumInfoes", "Klijent_Id", "dbo.AspNetUsers");
            DropIndex("dbo.PremiumInfoes", new[] { "Klijent_Id" });
            DropTable("dbo.PremiumInfoes");
        }
    }
}

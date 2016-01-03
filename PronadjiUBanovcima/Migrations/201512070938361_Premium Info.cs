namespace PronadjiUBanovcima.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PremiumInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PremiumInfoes", "Ime", c => c.String());
            AddColumn("dbo.PremiumInfoes", "Prezime", c => c.String());
            AddColumn("dbo.PremiumInfoes", "Email", c => c.String());
            AddColumn("dbo.PremiumInfoes", "Telefon", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PremiumInfoes", "Telefon");
            DropColumn("dbo.PremiumInfoes", "Email");
            DropColumn("dbo.PremiumInfoes", "Prezime");
            DropColumn("dbo.PremiumInfoes", "Ime");
        }
    }
}

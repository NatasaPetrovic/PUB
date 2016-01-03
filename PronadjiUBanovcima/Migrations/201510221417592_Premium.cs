namespace PronadjiUBanovcima.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Premium : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Infoes", "Opis");
            DropColumn("dbo.Infoes", "Sajt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Infoes", "Sajt", c => c.String());
            AddColumn("dbo.Infoes", "Opis", c => c.String());
        }
    }
}

namespace PronadjiUBanovcima.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConfEmail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Email", c => c.String());
            AddColumn("dbo.AspNetUsers", "ConfirmationToken", c => c.String());
            AddColumn("dbo.AspNetUsers", "IsConfirmed", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "IsConfirmed");
            DropColumn("dbo.AspNetUsers", "ConfirmationToken");
            DropColumn("dbo.AspNetUsers", "Email");
        }
    }
}

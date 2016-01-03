namespace PronadjiUBanovcima.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Delatnosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Naziv = c.String(),
                        IsSelected = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Infoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Firma = c.String(),
                        Opis = c.String(),
                        Telefon = c.String(),
                        Adresa = c.String(),
                        Sajt = c.String(),
                        Email = c.String(),
                        Klijent_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Klijent_Id)
                .Index(t => t.Klijent_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Slikas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Putanja = c.String(),
                        Alt = c.String(),
                        KlijentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Infoes", t => t.KlijentId, cascadeDelete: true)
                .Index(t => t.KlijentId);
            
            CreateTable(
                "dbo.InfoDelatnosts",
                c => new
                    {
                        Info_Id = c.Int(nullable: false),
                        Delatnost_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Info_Id, t.Delatnost_Id })
                .ForeignKey("dbo.Infoes", t => t.Info_Id, cascadeDelete: true)
                .ForeignKey("dbo.Delatnosts", t => t.Delatnost_Id, cascadeDelete: true)
                .Index(t => t.Info_Id)
                .Index(t => t.Delatnost_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Slikas", "KlijentId", "dbo.Infoes");
            DropForeignKey("dbo.InfoDelatnosts", "Delatnost_Id", "dbo.Delatnosts");
            DropForeignKey("dbo.InfoDelatnosts", "Info_Id", "dbo.Infoes");
            DropForeignKey("dbo.Infoes", "Klijent_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Slikas", new[] { "KlijentId" });
            DropIndex("dbo.InfoDelatnosts", new[] { "Delatnost_Id" });
            DropIndex("dbo.InfoDelatnosts", new[] { "Info_Id" });
            DropIndex("dbo.Infoes", new[] { "Klijent_Id" });
            DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropTable("dbo.InfoDelatnosts");
            DropTable("dbo.Slikas");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Infoes");
            DropTable("dbo.Delatnosts");
        }
    }
}

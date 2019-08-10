namespace anipet.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FoodSources",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        SourcePricePerKilo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ProductWeightInKilo = c.Int(nullable: false),
                        FoodSource_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FoodSources", t => t.FoodSource_Id, cascadeDelete: true)
                .Index(t => t.FoodSource_Id);
            
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        City = c.String(nullable: false),
                        StreetAddress = c.String(nullable: false),
                        StoreManager_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.StoreManager_Id, cascadeDelete: true)
                .Index(t => t.StoreManager_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Password = c.String(nullable: false),
                        IsAdmin = c.Boolean(nullable: false),
                        Username = c.String(nullable: false),
                        FavoriteProduct_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.FavoriteProduct_Id)
                .Index(t => t.FavoriteProduct_Id);
            
            CreateTable(
                "dbo.Purchases",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PurchaseId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Product_Id = c.Int(nullable: false),
                        User_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.Product_Id, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.Product_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.StoreProducts",
                c => new
                    {
                        Store_Id = c.Int(nullable: false),
                        Product_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Store_Id, t.Product_Id })
                .ForeignKey("dbo.Stores", t => t.Store_Id, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.Product_Id, cascadeDelete: true)
                .Index(t => t.Store_Id)
                .Index(t => t.Product_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Purchases", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Purchases", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.Stores", "StoreManager_Id", "dbo.Users");
            DropForeignKey("dbo.Users", "FavoriteProduct_Id", "dbo.Products");
            DropForeignKey("dbo.StoreProducts", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.StoreProducts", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.Products", "FoodSource_Id", "dbo.FoodSources");
            DropIndex("dbo.StoreProducts", new[] { "Product_Id" });
            DropIndex("dbo.StoreProducts", new[] { "Store_Id" });
            DropIndex("dbo.Purchases", new[] { "User_Id" });
            DropIndex("dbo.Purchases", new[] { "Product_Id" });
            DropIndex("dbo.Users", new[] { "FavoriteProduct_Id" });
            DropIndex("dbo.Stores", new[] { "StoreManager_Id" });
            DropIndex("dbo.Products", new[] { "FoodSource_Id" });
            DropTable("dbo.StoreProducts");
            DropTable("dbo.Purchases");
            DropTable("dbo.Users");
            DropTable("dbo.Stores");
            DropTable("dbo.Products");
            DropTable("dbo.FoodSources");
        }
    }
}

namespace anipet.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class with_newcontrollersandviews : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Stores", name: "StoreManager_Id", newName: "StoreAdmin_Id");
            RenameIndex(table: "dbo.Stores", name: "IX_StoreManager_Id", newName: "IX_StoreAdmin_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Stores", name: "IX_StoreAdmin_Id", newName: "IX_StoreManager_Id");
            RenameColumn(table: "dbo.Stores", name: "StoreAdmin_Id", newName: "StoreManager_Id");
        }
    }
}

using anipet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace anipet.DAL
{
    public class AnipetInitializer : System.Data.Entity.DropCreateDatabaseAlways<DBContext>
    {
        protected override void Seed(DBContext context)
        {
            var food_source = new List<FoodSource>
            {
                new FoodSource{Name="Vegan", SourcePricePerKilo=120, ProductsFromFoodSource=null, Id=1},
                new FoodSource{Name="Meat", SourcePricePerKilo=300, ProductsFromFoodSource=null, Id=2},
                new FoodSource{Name="DryFood", SourcePricePerKilo=750, ProductsFromFoodSource=null, Id=3},
                new FoodSource{Name="Fish", SourcePricePerKilo=2, ProductsFromFoodSource=null, Id=4},
                new FoodSource{Name="Chicken", SourcePricePerKilo=2, ProductsFromFoodSource=null, Id=5},
            };

            food_source.ForEach(s => context.FoodSources.Add(s));
            context.SaveChanges();

            var products = new List<Product>
            {
                new Product{Name="Brokoli", ProductWeightInKilo=15, FoodSource=context.FoodSources.Find(1), Id=1},
                new Product{Name="Bonzo", ProductWeightInKilo=98, FoodSource=context.FoodSources.Find(2), Id=2},
                new Product{Name="Trovet", ProductWeightInKilo=120, FoodSource=context.FoodSources.Find(2), Id=3},
                new Product{Name="Salmon", ProductWeightInKilo=360, FoodSource=context.FoodSources.Find(4), Id=4},
                new Product{Name="Waffle", ProductWeightInKilo=2, FoodSource=context.FoodSources.Find(3), Id=5},
            };
            products.ForEach(s => context.Products.Add(s));
            context.SaveChanges();

            var users = new List<User>
            {
                new User{Id=1,IsAdmin=true,Password="123",Username="Shai",FavoriteProduct=context.Products.Find(3),Stores=new List<Store>{context.Stores.Find(1)} },
                new User{Id=2,IsAdmin=true,Password="HelloWorld1!",Username="Guy",FavoriteProduct=context.Products.Find(1), Stores=new List<Store>{context.Stores.Find(1)}},
                new User{Id=3,IsAdmin=true,Password="Ashkenazi",Username="Alon",FavoriteProduct=context.Products.Find(4), Stores=new List<Store>{context.Stores.Find(1)}},
                new User{Id=4,IsAdmin=true,Password="BarRefaeli",Username="Maor",FavoriteProduct=context.Products.Find(5), Stores=new List<Store>{context.Stores.Find(1)}},
                new User{Id=5,IsAdmin=false,Password="Losers",Username="Hapoel",FavoriteProduct=context.Products.Find(5), Stores=new List<Store>{context.Stores.Find(1)}},
            };
            users.ForEach(s => context.Users.Add(s));
            context.SaveChanges();
            context.SaveChanges();

            var purchases = new List<Purchase>
            {
                new Purchase{Id=1,PurchaseId=1, Product=context.Products.Find(2),User=context.Users.Find(4),Date=new DateTime(2015, 3, 1)},
                new Purchase{Id=2,PurchaseId=2, Product=context.Products.Find(1),User=context.Users.Find(3),Date=new DateTime(2016, 6, 3)},
                new Purchase{Id=3,PurchaseId=3, Product=context.Products.Find(4),User=context.Users.Find(3),Date=new DateTime(2016, 6, 4)},
                new Purchase{Id=4,PurchaseId=4, Product=context.Products.Find(3),User=context.Users.Find(2),Date=new DateTime(2016, 6, 8)},
                new Purchase{Id=5,PurchaseId=5, Product=context.Products.Find(4),User=context.Users.Find(5),Date=new DateTime(2016, 7, 8)},
            };
            purchases.ForEach(s => context.Purchases.Add(s));
            context.SaveChanges();
            context.SaveChanges();


            var stores = new List<Store>
            {
                new Store{City="Rishon Lezion", StreetAddress="Rotshild 22", StoreAdmin=context.Users.Find(1), Id=1, Products=new List<Product>{context.Products.Find(2), context.Products.Find(4), context.Products.Find(1) } },
                new Store{City="NewYork", StreetAddress="boulevard of broken dreams 4", StoreAdmin=context.Users.Find(3), Id=2, Products=new List<Product>{context.Products.Find(1), context.Products.Find(4) } },
                new Store{City="Eilat", StreetAddress="Ahla 12", StoreAdmin=context.Users.Find(3), Id=3, Products=new List<Product>{context.Products.Find(1), context.Products.Find(5), context.Products.Find(3), context.Products.Find(2) } },
            };
            stores.ForEach(s => context.Stores.Add(s));
            context.SaveChanges();

        }
    }
}
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        private static string ResultDirectoryPath = "../../../Datasets/Result";
        public static void Main(string[] args)
        {
            ProductShopContext db = new ProductShopContext();

            //string inputJson = File.ReadAllText("../../../Datasets/categories-products.json");

            var json = GetUsersWithProducts(db);
            //if (!Directory.Exists(ResultDirectoryPath))
            //{
            //    Directory.CreateDirectory(ResultDirectoryPath);
            //}
            File.WriteAllText(ResultDirectoryPath + "/users-and-products.json", json);



        }

        private static void ResetDatabase(ProductShopContext db)
        {
            db.Database.EnsureDeleted();
            Console.WriteLine("Database is deleted");
            db.Database.EnsureCreated();
            Console.WriteLine("Database is created");
        }
        //Problem 1
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            User[] users = JsonConvert.DeserializeObject<User[]>(inputJson);
            context.AddRange(users);
            context.SaveChanges();
            return $"Successfully imported {users.Length}";
        }
        //Problem 2
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            Product[] products = JsonConvert.DeserializeObject<Product[]>(inputJson);
            context.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }
        //Problem 3
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {

            Category[] categories = JsonConvert.DeserializeObject<Category[]>(inputJson).Where(x => x.Name != null).ToArray();
            context.AddRange(categories);
            context.SaveChanges();
            return $"Successfully imported {categories.Length}";
        }
        //Problem 4
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            CategoryProduct[] categoriProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);
            context.AddRange(categoriProducts);
            context.SaveChanges();
            return $"Successfully imported {categoriProducts.Length}";
        }
        //Problem 5
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products

                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(x => new
                {
                    name = x.Name,
                    price = x.Price,
                    seller = x.Seller.FirstName + " " + x.Seller.LastName
                }).ToList();
            string json = JsonConvert.SerializeObject(products, Formatting.Indented);
            return json;
        }
        //Problem 6
        public static string GetSoldProducts(ProductShopContext context)
        {
            var allUsers = context.Users.Where(x => x.ProductsSold.Any(x => x.Buyer != null))
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    soldProducts = x.ProductsSold.Where(p => p.Buyer != null)
                        .Select(x => new
                        {
                            name = x.Name,
                            price = x.Price,
                            buyerFirstName = x.Buyer.FirstName,
                            buyerLastName = x.Buyer.LastName,
                        }).ToArray()
                }).ToList();
            string json = JsonConvert.SerializeObject(allUsers, Formatting.Indented);
            return json;
        }
        //Problem 7
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(x => new
                {
                    category = x.Name,
                    productsCount = x.CategoryProducts.Count,
                    averagePrice = x.CategoryProducts
                        .Average(x=>x.Product.Price)
                        .ToString("f2"),
                    totalRevenue = x.CategoryProducts
                        .Sum(x=>x.Product.Price)
                        .ToString("f2")
                }).OrderByDescending(x=>x.productsCount)
                .ToList();
            var json = JsonConvert.SerializeObject(categories, Formatting.Indented);
            return json;
        }
        //Problem 8
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context
                .Users
                .Where(u => u.ProductsSold.Count >= 1).ToList()
                .Select(u => new
                {
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = new
                    {
                        count = u.ProductsSold.Count(ps => ps.Buyer != null),
                        products = u.ProductsSold
                            .Where(ps => ps.Buyer != null)
                            .Select(ps => new
                            {
                                name = ps.Name,
                                price = ps.Price
                            })
                            .ToList()
                    },
                })
                .ToList()
                .OrderByDescending(u => u.soldProducts.count)
                .ToList();

            var resultObj = new
            {
                usersCount = users.Count,
                users = users
            };

            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented

            };

            var json = JsonConvert.SerializeObject(resultObj, settings);

            return json;
        }

    }
}
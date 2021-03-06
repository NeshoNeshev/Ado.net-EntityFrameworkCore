﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using ProductShop.Data;
using ProductShop.Models;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;

namespace ProductShop
{
    public class StartUp
    {
        private const string DatasetDirectoryPath = @"../../../Datasets/";
        private const string ResultsDirPath = DatasetDirectoryPath + "Results/";
        public static void Main(string[] args)
        {
            ProductShopContext db = new ProductShopContext();
            //ResetDatabase(db);
            InitializeMapper();

        }


        public static string GetUsersWithProducts(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            var users = new UserRootDTO()
            {
                Count = context.Users.Count(u => u.FirstName != null),
                Users = context.Users
                    .Where(u => u.ProductsSold.Count >= 1)
                    .OrderByDescending(u => u.ProductsSold.Count)
                    .Select(u => new UserExportDTO()
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Age = u.Age,
                        SoldProducts = new SoldProductsDTO()
                        {
                            Count = u.ProductsSold.Count(ps => ps.Buyer != null),
                            Products = u.ProductsSold
                                .Where(ps => ps.Buyer != null)
                                .Select(ps => new ExportProductSoldDTO()
                                {
                                    Name = ps.Name,
                                    Price = ps.Price
                                })
                                .OrderByDescending(p=>p.Price)
                                .Take(10)
                                .ToArray()
                        }
                    })
                    .ToArray()
            };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserRootDTO), new XmlRootAttribute("Users"));

            xmlSerializer.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString().Trim();
        }


        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            var categories = context
                .Categories
                .ProjectTo<ExportCategoryByProductsDTO>()
                .OrderByDescending(c => c.ProductsCount)
                .ThenBy(c => c.TotalRevenue)
                .ToArray();

            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ExportCategoryByProductsDTO[]), new XmlRootAttribute("Categories"));

            xmlSerializer.Serialize(new StringWriter(sb), categories, namespaces);

            return sb.ToString().Trim();


        }

 
        public static string GetSoldProducts(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            var users = context
                .Users
                .Where(u => u.ProductsSold.Count >= 1)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ProjectTo<ExportUserSoldProductsDTO>()
                .ToArray();

            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ExportUserSoldProductsDTO[]), new XmlRootAttribute("Users"));

            xmlSerializer.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString().Trim();
        }

 
        public static string GetProductsInRange(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            var products = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .ProjectTo<ExportProductsInRangeDTO>()
                .ToArray();

            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ExportProductsInRangeDTO[]), new XmlRootAttribute("Products"));

            xmlSerializer.Serialize(new StringWriter(sb), products, namespaces);

            return sb.ToString().Trim();
        }

 
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCategoryProductDTO[]),
                new XmlRootAttribute("CategoryProducts"));

            var categoryProductsDtos =
                (ImportCategoryProductDTO[]) xmlSerializer.Deserialize(new StringReader(inputXml));

            var categoryProducts = Mapper.Map<CategoryProduct[]>(categoryProductsDtos.Where(cp =>
                context.Categories.Any(c => c.Id == cp.CategoryId) && context.Products.Any(p => p.Id == cp.ProductId)));

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Length}";

        }

 
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ImportCategoryDTO[]), new XmlRootAttribute("Categories"));

            var categoriesDtos = (ImportCategoryDTO[]) xmlSerializer.Deserialize(new StringReader(inputXml));

            var categories = Mapper.Map<Category[]>(categoriesDtos.Where(c => c.Name != null));

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Length}";

        }

 
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ImportProductDTO[]), new XmlRootAttribute("Products"));

            var productsDtodDtos = (ImportProductDTO[])xmlSerializer.Deserialize(new StringReader(inputXml));

            List<Product> products = new List<Product>();

            foreach (var productDto in productsDtodDtos)
            {
                Product product = new Product()
                {
                    BuyerId = productDto.BuyerId,
                    Name = productDto.Name,
                    SellerId = productDto.SellerId,
                    Price = productDto.Price
                };
                products.Add(product);
            }

            //var products = Mapper.Map<Product[]>(productsDtodDtos);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";

        }

 
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportUserDTO[]), new XmlRootAttribute("Users"));

            var usersDtos = (ImportUserDTO[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var users = Mapper.Map<User[]>(usersDtos);
            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Length}";

        }

        private static void InitializeMapper()
        {
            Mapper.Initialize(cfg => { cfg.AddProfile<ProductShopProfile>(); });
        }

        private static void ResetDatabase(ProductShopContext db)
        {
            db.Database.EnsureDeleted();
            Console.WriteLine("Database is successfully deleted");
            db.Database.EnsureCreated();
            Console.WriteLine("Database is successfully created");
        }
    }
}
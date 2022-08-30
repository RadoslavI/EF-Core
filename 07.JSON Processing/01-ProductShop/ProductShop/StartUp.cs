using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs.Category;
using ProductShop.DTOs.Category_Product;
using ProductShop.DTOs.Product;
using ProductShop.DTOs.User;
using ProductShop.DTOs.Users;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)  
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            ProductShopContext dbContext = new ProductShopContext();
            //string inputJSON = File.ReadAllText("../../../DataSets/categories-products.json");
            string outputJSON = "../../../Results/users-and-products.json";

            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();

            //Console.WriteLine("Database copy was successful!");

            //Problems 01 - 04
            //Console.WriteLine(ImportCategoryProducts(dbContext, inputJSON));

            string json = GetUsersWithProducts(dbContext);
            File.WriteAllText(outputJSON, json);
        }

        //Problem 01
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var userDTOs = JsonConvert.DeserializeObject<ImportUserDTO[]>(inputJson);

            ICollection<User> users = new List<User>();

            foreach (var uDto in userDTOs)
            {
                var user = Mapper.Map<User>(uDto);
                users.Add(user);
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        //Problem 02
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var productDTOs = JsonConvert.DeserializeObject<ImportProductDTO[]>(inputJson);

            ICollection<Product> products = new List<Product>();

            foreach (var pDto in productDTOs)
            {
                var product = Mapper.Map<Product>(pDto);
                products.Add(product);
            }

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        //Problem 03
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categoryDTOs = JsonConvert.DeserializeObject<ImportCategoryDTO[]>(inputJson);

            ICollection<Category> categories = new List<Category>();

            foreach (var cDto in categoryDTOs)
            {
                var category = Mapper.Map<Category>(cDto);
                if (category.Name != null)
                { 
                    categories.Add(category);
                }
            }

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        //Problem 04
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryProductDTOs = JsonConvert.DeserializeObject<ImportCategoryProductDTO[]>(inputJson);

            ICollection<CategoryProduct> categoryProducts = new List<CategoryProduct>();

            foreach (var cDto in categoryProductDTOs)
            {
                var category = Mapper.Map<CategoryProduct>(cDto);
                categoryProducts.Add(category);
            }

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        //Problem 05
        public static string GetProductsInRange(ProductShopContext context)
        {
            ExportProductsInRangeDTO[] products = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .ProjectTo<ExportProductsInRangeDTO>()
                .ToArray();

            string json = JsonConvert.SerializeObject(products, Formatting.Indented);

            return json;
        }

        //Problem 06
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context
                .Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId.HasValue))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.LastName)
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold
                    .Where(p => p.BuyerId.HasValue)
                    .Select(p => new
                    {
                        name = p.Name,
                        price = p.Price,
                        buyerFirstName = p.Buyer.FirstName,
                        buyerLastName = p.Buyer.LastName
                    })
                    .ToArray()
                })
                .ToArray();

            return JsonConvert.SerializeObject(users, Formatting.Indented);
        }

        //Problem 07
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            //var users = context
            //    .Users
            //    .Where(u => u.ProductsSold.Any(p => p.BuyerId.HasValue))
            //    .OrderByDescending(u => u.ProductsSold.Count(p => p.BuyerId.HasValue))
            //    .ProjectTo<ExportUsersWithFullProductInfoDto>()
            //    .ToArray();

            ExportUsersInfoDto serDto = new ExportUsersInfoDto()
            {
                Users = context
                    .Users
                    .Where(u => u.ProductsSold.Any(p => p.BuyerId.HasValue))
                    .OrderByDescending(u => u.ProductsSold.Count(p => p.BuyerId.HasValue))
                    .Select(u => new ExportUsersWithFullProductInfoDto()
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Age = u.Age,
                        SoldProductsInfo = new ExportProductsFullInfoDto()
                        {
                            SoldProducts = u.ProductsSold
                            .Where(p => p.BuyerId.HasValue)
                            .Select(p => new ExportSoldProductShortInfoDto()
                            {
                                Name = p.Name,
                                Price = p.Price
                            })
                            .ToArray()
                        }
                    })
                    .ToArray()

            };

            var serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(serDto, Formatting.Indented, serializerSettings);
        }
    }
}
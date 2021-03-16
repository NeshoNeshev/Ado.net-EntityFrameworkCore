using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        private static string ResultDirectoryPath = "../../../Datasets/Result";

        private static void InitializeMapper()
        {

            Mapper.Initialize(cfg => { cfg.AddProfile<CarDealerProfile>(); });
        }

        public static void Main(string[] args)
        {
            InitializeMapper();
            var db = new CarDealerContext();
            var json = GetSalesWithAppliedDiscount(db);
            
           File.WriteAllText(ResultDirectoryPath + "/sales-discounts.json", json);

        }

        private static void ResetDatabase(CarDealerContext db)
        {
            db.Database.EnsureDeleted();
            Console.WriteLine("Database is deleted");
            db.Database.EnsureCreated();
            Console.WriteLine("Database is created");
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);
            context.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {

            var parts = JsonConvert.DeserializeObject<List<Part>>(inputJson)
                .Where(p =>
                    Enumerable.Range(context.Suppliers.Min(x => x.Id),
                            context.Suppliers.Max(x => x.Id))
                        .Contains(p.SupplierId))
                .ToList();
            context.AddRange(parts);

            context.SaveChanges();

            return $"Successfully imported {parts.Count}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            List<ImportCarDto> carsDtos = JsonConvert.DeserializeObject<List<ImportCarDto>>(inputJson);

            List<Car> cars = new List<Car>();
            foreach (var carDto in carsDtos)
            {
                Car car = new Car()
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TravelledDistance
                };

                foreach (int partId in carDto.partsId.Distinct())
                {
                    car.PartCars.Add(new PartCar()
                    {
                        Car = car,
                        PartId = partId
                    });
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson);
            context.AddRange(customers);
            context.SaveChanges();
            return $"Successfully imported {customers.Count}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson);
            context.AddRange(sales);
            context.SaveChanges();
            return $"Successfully imported {sales.Count}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToList();

            string result = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return result;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars.Where(c => c.Make == "Toyota")
                .Select(x => new
                {
                    Id = x.Id,
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ToList();
            string result = JsonConvert.SerializeObject(cars, Formatting.Indented);
            return result;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count
                }).ToList();
            string result = JsonConvert.SerializeObject(suppliers, Formatting.Indented);
            return result;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(x => new
                {
                    car = new
                    {
                        Make = x.Make,
                        Model = x.Model,
                        TravelledDistance = x.TravelledDistance
                    },
                    parts = x.PartCars.Select(pc => new
                        {
                            Name = pc.Part.Name,
                            Price = pc.Part.Price.ToString("f2")
                        })
                        .ToList()
                })
                .ToList();
            string result = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return result;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context
                .Customers
                .ProjectTo<CustomerTotalSalesDTO>()
                .Where(c => c.CarsBought >= 1)
                .OrderByDescending(c => c.SpentMoney)
                .ThenByDescending(c => c.CarsBought)
                .ToList();

            string result = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return result;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context
                .Sales
                .Select(s => new CustomerSaleDTO()
                {
                    Car = new SaleDTO()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },
                    CustomerName = s.Customer.Name,
                    Discount = s.Discount.ToString("f2"),
                    Price = s.Car.PartCars.Sum(pc => pc.Part.Price).ToString("f2"),
                    PriceWithDiscount = (s.Car.PartCars.Sum(pc => pc.Part.Price) -
                                         s.Car.PartCars.Sum(pc => pc.Part.Price) * s.Discount / 100).ToString("f2")

                })
                .Take(10)
                .ToList();

            var json = JsonConvert.SerializeObject(sales, Formatting.Indented);

            return json;
        }
    }
}
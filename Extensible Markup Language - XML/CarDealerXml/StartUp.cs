using AutoMapper;
using CarDealer.Data;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper.QueryableExtensions;
using CarDealer.Dtos.Export;
using CarDealer.XMLHelper;


namespace CarDealer
{
    public class StartUp
    {
        private const string DatasetsDirPath = @"../../../Datasets/";
        private const string ResultDirPath = DatasetsDirPath + "Results/";


        public static void Main(string[] args)
        {
            using CarDealerContext db = new CarDealerContext();
            ResetDatabase(db);

            string result = GetSalesWithAppliedDiscount(db);
            File.WriteAllText(ResultDirPath + "sales-discounts.xml", result);

        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context
                .Sales
                .Select(s => new ExportSaleDTO()
                {
                    Discount = s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartCars.Sum(pc => pc.Part.Price),
                    PriceWithDiscount = s.Car.PartCars.Sum(pc => pc.Part.Price) -
                                        s.Car.PartCars.Sum(pc => pc.Part.Price) * s.Discount / 100,
                    Car = new ExportCarSaleDTO()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },
                })
                .ToList();
            var result = XmlConverter.Serialize(sales, "sales");
            return result;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {

            var customers = context
                .Customers
                .Where(x => x.Sales.Count >= 1)
                .Select(x => new ExportCustomerTotalSalesDTO
                {
                    Name = x.Name,
                    BoughtCars = x.Sales.Count,
                    SpentMoney = x.Sales
                        .Select(s => s.Car.PartCars.
                            Sum(pc => pc.Part.Price))
                        .Sum()
                }).ToList()

                .OrderByDescending(x => x.SpentMoney)
                .ToList();

            var result = XmlConverter.Serialize(customers, "customers");

            return result;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {

            var cars = context.Cars
                .Select(x => new ExportCarWithPartsDTO
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance,
                    Parts = x.PartCars.Select(x => new ExportPartCarsDTO
                    {
                        Name = x.Part.Name,
                        Price = x.Part.Price
                    }).OrderByDescending(x => x.Price).ToArray()
                })
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ToArray();

            var result = XmlConverter.Serialize(cars, "cars");

            return result;

        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {

            var suppliers = context
                .Suppliers
                .Where(s => !s.IsImporter)
                .Select(x => new ExportLocalSuppliersDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count
                })

                .ToList();

            var result = XmlConverter.Serialize(suppliers, "suppliers");

            return result;
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context
                .Cars
                .Where(c => c.Make == "BMW")
                .Select(x => new ExportCarsBMWDTO
                {
                    Id = x.Id,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance

                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)

                .ToList();

            var result = XmlConverter.Serialize(cars, "cars");

            return result;
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {

            var cars = context.Cars
                .Where(c => c.TravelledDistance > 2_000_000)
                .Select(x => new ExportCarWithDistanceDTO
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(m => m.Make)
                .ThenBy(ml => ml.Model)
                .Take(10)

                .ToList();

            var result = XmlConverter.Serialize(cars, "cars");

            return result;
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = XmlConverter.Deserializer<ImportSaleDTO>(inputXml, "Sales");

            var salesDto = xmlSerializer
                .Where(s => context.Cars.Any(c => c.Id == s.CarId))
                .Select(s => new Sale
                {
                    CarId = s.CarId,
                    CustomerId = s.CustomerId,
                    Discount = s.Discount

                }).ToList();

            context.Sales.AddRange(salesDto);
            context.SaveChanges();

            return $"Successfully imported {salesDto.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = XmlConverter.Deserializer<ImportCustomerDTO>(inputXml, "Customers");

            var customersDto = xmlSerializer.Select(c => new Customer
            {
                Name = c.Name,
                BirthDate = c.BirthDate,
                IsYoungDriver = c.IsYoungDriver
            }).ToList();

            context.Customers.AddRange(customersDto);
            context.SaveChanges();

            return $"Successfully imported {customersDto.Count}";

        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = XmlConverter.Deserializer<ImportCarsDTO>(inputXml, "Cars");

            List<Car> cars = new List<Car>();
            List<PartCar> partCars = new List<PartCar>();

            var carsDto = xmlSerializer;

            foreach (var itemCarsDto in carsDto)
            {
                var car = new Car()
                {
                    Make = itemCarsDto.Make,
                    Model = itemCarsDto.Model,
                    TravelledDistance = itemCarsDto.TravelledDistance
                };
                var parts = itemCarsDto
                        .Parts
                        .Where(pc => context.Parts.Any(p => p.Id == pc.Id))
                        .Select(p => p.Id)
                        .Distinct();
                foreach (var part in parts)
                {
                    PartCar partCar = new PartCar()
                    {
                        PartId = part,
                        Car = car
                    };

                    partCars.Add(partCar);
                }

                cars.Add(car);
            }

            context.PartCars.AddRange(partCars);
            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = XmlConverter.Deserializer<ImportPartsDTO>(inputXml, "Parts");

            var partsDto = xmlSerializer
                .Where(s => context.Suppliers
                    .Any(x => x.Id == s.SupplierId))
                .Select(p => new Part
                {
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    SupplierId = p.SupplierId
                }).ToArray();

            context.Parts.AddRange(partsDto);
            context.SaveChanges();

            return $"Successfully imported {partsDto.Length}";
        }


        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = XmlConverter.Deserializer<ImportSupplierDTO>(inputXml, "Suppliers");

            var suppliersDtos = xmlSerializer.Select(x => new Supplier
            {
                Name = x.Name,
                IsImporter = x.IsImporter
            }).ToList();

            context.Suppliers.AddRange(suppliersDtos);
            context.SaveChanges();

            return $"Successfully imported {suppliersDtos.Count}";
        }

        private static void ResetDatabase(CarDealerContext db)
        {
            db.Database.EnsureDeleted();
            Console.WriteLine("Database successfully deleted!");
            db.Database.EnsureCreated();
            Console.WriteLine("Database successfully created!");
        }
    }
}
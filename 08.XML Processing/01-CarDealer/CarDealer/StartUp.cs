using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {

        public static void Main(string[] args)
        {
            CarDealerContext dbContext = new CarDealerContext();

            //string inputXml = File.ReadAllText("../../../Datasets/sales.xml");


            Console.WriteLine(GetTotalSalesByCustomer(dbContext));
            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();
            //Console.WriteLine("Database reset successfully!");


        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Suppliers");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSupplierDto[]), xmlRoot);

            using StringReader reader = new StringReader(inputXml);
            ImportSupplierDto[] supplierDtos = (ImportSupplierDto[])xmlSerializer.Deserialize(reader);

            Supplier[] suppliers = supplierDtos
                .Select(dt => new Supplier
                {
                    Name = dt.Name,
                    IsImporter = dt.IsImporter
                })
                .ToArray();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var partsDtos = Deserialize<ImportPartDto[]>(inputXml, "Parts");

            ICollection<Part> parts = new List<Part>();
            foreach (var pDto in partsDtos)
            {
                if (context.Suppliers.Any(s => s.Id == pDto.SupplierId))
                {
                    Part part = new Part()
                    {
                        Name = pDto.Name,
                        Price = pDto.Price,
                        Quantity = pDto.Quantity,
                        SupplierId = pDto.SupplierId
                    };
                    parts.Add(part);
                }
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var carsDtos = Deserialize<ImportCarDto[]>(inputXml, "Cars");

            Car[] cars = new Car[carsDtos.Length];
            //An example of manual mapping of import json
            for (int i = 0; i < carsDtos.Length; i++)
            {
                Car newCar = new Car
                {
                    Make = carsDtos[i].Make,
                    Model = carsDtos[i].Model,
                    TravelledDistance = carsDtos[i].TraveledDistance,
                };

                foreach (int partId in carsDtos[i].Parts.Select(p => p.Id).Distinct())
                {
                    if (context.Parts.Any(p => p.Id == partId))
                    {
                        newCar.PartCars.Add(new PartCar
                        {
                            PartId = partId
                        });
                    }
                }

                cars[i] = newCar;
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Length}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var customersDtos = Deserialize<ImportCustomerDto[]>(inputXml, "Customers");

            ICollection<Customer> customers = new List<Customer>();
            foreach (var cDto in customersDtos)
            {

                Customer customer = new Customer()
                {
                    Name = cDto.Name,
                    BirthDate = DateTime.Parse(cDto.BirthDate, CultureInfo.InvariantCulture),
                    IsYoungDriver = cDto.IsYoungDriver
                };
                customers.Add(customer);

            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var salesDtos = Deserialize<ImportSaleDto[]>(inputXml, "Sales");

            ICollection<Sale> sales = new List<Sale>();
            foreach (var sDto in salesDtos)
            {
                if (context.Cars.Any(s => s.Id == sDto.CarId))
                {
                    Sale sale = new Sale()
                    {
                        CarId = sDto.CarId,
                        CustomerId = sDto.CustomerId,
                        Discount = sDto.Discount
                    };
                    sales.Add(sale);
                }
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            var carDtos = context
                .Cars
                .Where(c => c.TravelledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .Select(c => new ExportCarsWithDistanceDto()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToArray();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("cars");
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCarsWithDistanceDto[]), xmlRoot);
            
            using StringWriter writer = new StringWriter(sb);
            xmlSerializer.Serialize(writer, carDtos, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var carDtos = context
                .Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new ExportCarInfoDto()
                {
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToArray();

            return Serialize<ExportCarInfoDto[]>("cars", carDtos);
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var supplierDtos = context
                .Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new ExportSuppliersDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count()
                })
                .ToArray();

            return Serialize<ExportSuppliersDto[]>("suppliers", supplierDtos);
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsDto = context
                .Cars
                .Select(c => new ExportCarWithPartsInfoDto()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars
                    .Select(pc => new ExportCarPartsDto()
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price
                    })
                    .OrderByDescending(pc => pc.Price)
                    .ToArray()
                })
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ToArray();

            return Serialize<ExportCarWithPartsInfoDto[]>("cars", carsDto);
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customersDto = context
                .Customers
                .Where(c => c.Sales.Count() >= 1)
                .Select(c => new ExportCustomersDto()
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count(),
                    SpentMoney = c.Sales
                       .Select(s => s.Car)
                       .SelectMany(c => c.PartCars)
                       .Sum(pt => pt.Part.Price)
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToArray();


            return Serialize<ExportCustomersDto[]>("customers", customersDto);
        }

        private static T Deserialize<T>(string inputXml, string rootName)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

            using StringReader reader = new StringReader(inputXml);
            T dtos = (T)xmlSerializer.Deserialize(reader);

            return dtos;
        }

        private static string Serialize<T>(string rootName, T dto)
        {
            StringBuilder sb = new StringBuilder();
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

            using StringWriter writer = new StringWriter(sb);
            xmlSerializer.Serialize(writer, dto, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}
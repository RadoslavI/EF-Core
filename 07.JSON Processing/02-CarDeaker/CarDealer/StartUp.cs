using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO.Export;
using CarDealer.DTO.Import;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });
            CarDealerContext dbContext = new CarDealerContext();
            string inputJson = File.ReadAllText("../../../DataSets/sales.json");

            string path = "../../../Results/cars-and-parts.json";

            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();
            //Console.WriteLine("Database copy was successful!");

            string outputJson = GetCarsWithTheirListOfParts(dbContext);
            File.WriteAllText(path, outputJson);

            //Console.WriteLine(ImportSales(dbContext, inputJson));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var supplierDTOs = JsonConvert.DeserializeObject<ImportSuppliersDto[]>(inputJson);

            ICollection<Supplier> suppliers = new List<Supplier>();

            foreach (var sDto in supplierDTOs)
            {
                var supplier = Mapper.Map<Supplier>(sDto);
                suppliers.Add(supplier);
            }

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var partsDto = JsonConvert.DeserializeObject<ImportPartsDto[]>(inputJson);

            ICollection<Part> parts = new List<Part>();

            foreach (var Dto in partsDto)
            {

                var part = Mapper.Map<Part>(Dto);
                if (context.Suppliers.Any(s => s.Id == part.SupplierId))
                {
                    parts.Add(part);
                }
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            ImportCarsDto[] carDtos = JsonConvert.DeserializeObject<ImportCarsDto[]>(inputJson);

            Car[] cars = new Car[carDtos.Length];
            //An example of manual mapping of import json
            for (int i = 0; i < carDtos.Length; i++)
            {
                Car newCar = new Car
                {
                    Make = carDtos[i].Make,
                    Model = carDtos[i].Model,
                    TravelledDistance = carDtos[i].TravelledDistance,
                };

                foreach (int partId in carDtos[i].PartsId.Distinct())
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

            return $"Successfully imported {cars.Count()}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customerDtos = JsonConvert.DeserializeObject<ImportCustomerDto[]>(inputJson);

            ICollection<Customer> customers = new List<Customer>();

            foreach (var sDto in customerDtos)
            {
                var customer = Mapper.Map<Customer>(sDto);
                customers.Add(customer);
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            ImportSaleDto[] saleDtos = JsonConvert.DeserializeObject<ImportSaleDto[]>(inputJson);

            Sale[] sales = Mapper.Map<Sale[]>(saleDtos);
            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context
                .Customers
                .OrderBy(c => c.BirthDate)
                .ThenByDescending(c => c.IsYoungDriver)
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToArray();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context
                .Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToArray();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context
                .Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count()
                })
                .ToArray();

            return JsonConvert.SerializeObject(suppliers, Formatting.Indented);
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context
                .Cars
                .Select(c => new
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    parts = c.PartCars.Select(p => new
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price
                    }).ToArray()
                }).ToArray();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }
    }
}
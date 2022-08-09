namespace ORMFundamentals_LiveDemo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualBasic;

    internal class Program
    {
        static void Main(string[] args)
        {
            using SoftUniDbContext dbContext = new SoftUniDbContext();

            //int itDepId = dbContext
            //    .Departments
            //    .First(d => d.Name == "IT")
            //    .DepartmentID;

            int searchId = 2;
            Department dep2 = dbContext
                .Departments
                .Find(searchId);
            Console.WriteLine($"{dep2.DepartmentID}. {dep2.Name}");

            //Load entity in-memory
            //Employee peshoNotToBe = dbContext
            //    .Employees
            //    .First(e => e.FirstName == "Pesho");
            //Change entity in-memory
            //peshoNotToBe.FirstName = "Petar";
            //dbContext.SaveChanges(); //Update will begin

            Employee peshoGone = dbContext
                .Employees
                .First(e => e.FirstName == "Petar");
            dbContext.Employees.Remove(peshoGone);
            dbContext.SaveChanges(); //You have transaction

            //var itEmployees = dbContext
            //    .Employees
            //    .Where(e => e.Department.Name == "IT")
            //    .Select(e => new
            //    {
            //        e.FirstName,
            //        e.LastName,
            //        e.JobTitle
            //    })
            //    .ToList();
            int itEmployeesCnt = dbContext
                .Employees
                .Count(e => e.Department.Name == "IT");

            Console.WriteLine($"Department: IT - {itEmployeesCnt} Employees Available");
            //Exception
            //foreach (var employee in itEmployees)
            //{
            //    Console.WriteLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
            //}
        }
    }
}

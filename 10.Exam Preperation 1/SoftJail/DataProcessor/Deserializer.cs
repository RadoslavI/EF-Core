namespace SoftJail.DataProcessor
{
    using AutoMapper;
    using Data;
    using Microsoft.EntityFrameworkCore.Internal;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportDepartmentWithCellsDto[] departmentDtos =
                JsonConvert.DeserializeObject<ImportDepartmentWithCellsDto[]>(jsonString);

            ICollection<Department> validDepartments = new List<Department>();
            foreach (var dDto in departmentDtos)
            {
                if (!IsValid(dDto))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                if (!dDto.Cells.Any())
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                if (dDto.Cells.Any(c => !IsValid(c)))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Department department = new Department()
                {
                    Name = dDto.Name
                };

                foreach (var cellDto in dDto.Cells)
                {
                    var cell = Mapper.Map<Cell>(cellDto);
                    department.Cells.Add(cell);
                }

                validDepartments.Add(department);
                sb.AppendLine($"Imported {department.Name} with {department.Cells.Count()} cells");
            }

            context.Departments.AddRange(validDepartments);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportPrisonerWithMailsDto[] prisonerDtos =
                JsonConvert.DeserializeObject<ImportPrisonerWithMailsDto[]>(jsonString);

            ICollection<Prisoner> validPrisoners = new List<Prisoner>();
            foreach (var pDto in prisonerDtos)
            {
                bool isICDateValid =
                    DateTime.TryParseExact(pDto.IncarcerationDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime incarcerationDate);

                if (!IsValid(pDto) || pDto.Mails.Any(m => !IsValid(m)) || !isICDateValid)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                DateTime? releaseDate = null;
                if (!String.IsNullOrEmpty(pDto.ReleaseDate))
                {
                    bool isRDValid = DateTime.TryParseExact(pDto.ReleaseDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime releaseDateValue);

                    if (!isRDValid)
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }

                    releaseDate = releaseDateValue;
                }

                Prisoner prisoner = new Prisoner()
                {
                    FullName = pDto.FullName,
                    Nickname = pDto.Nickname,
                    Age = pDto.Age,
                    IncarcerationDate = incarcerationDate,
                    ReleaseDate = releaseDate,
                    Bail = pDto.Bail,
                    CellId = pDto.CellId
                };

                foreach (var mDto in pDto.Mails)
                {
                    Mail mail = Mapper.Map<Mail>(mDto);
                    prisoner.Mails.Add(mail);
                }

                validPrisoners.Add(prisoner);

                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
            }

            context.Prisoners.AddRange(validPrisoners);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Officers");
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ImportOfficersPrisonersDto[]), xmlRoot);

            using StringReader reader = new StringReader(xmlString);
            ImportOfficersPrisonersDto[] oDtos = (ImportOfficersPrisonersDto[])
                xmlSerializer.Deserialize(reader);

            ICollection<Officer> validOfficers = new List<Officer>();
            foreach (var oDto in oDtos)
            {
                if (!IsValid(oDto))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                bool isPositionEnumValid =
                    Enum.TryParse(typeof(Position), oDto.Position, out object posObj);

                if (!isPositionEnumValid)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                bool isWeaponEnumValid =
                    Enum.TryParse(typeof(Weapon), oDto.Weapon, out object wpnObj);

                if (!isWeaponEnumValid)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                //if (!context.Departments.Any(d => d.Id == oDto.DepartmentId))
                //{
                //    sb.AppendLine("Invalid Data");
                //    continue;
                //}

                Officer officer = new Officer()
                {
                    FullName = oDto.FullName,
                    Salary = oDto.Salary,
                    Position = (Position)posObj,
                    Weapon = (Weapon)wpnObj,
                    DepartmentId = oDto.DepartmentId
                };
                foreach (var pDto in oDto.Prisoners)
                {
                    OfficerPrisoner op = new OfficerPrisoner()
                    {
                        Officer = officer,
                        PrisonerId = pDto.Id
                    };
                    officer.OfficerPrisoners.Add(op);
                }

                validOfficers.Add(officer);
                sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
            }

            context.Officers.AddRange(validOfficers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}
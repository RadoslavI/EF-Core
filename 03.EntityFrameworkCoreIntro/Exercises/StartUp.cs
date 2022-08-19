using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext dbContext = new SoftUniContext();

            Console.WriteLine(RemoveTown(dbContext));
        }

        //Propblem 03
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            Employee[] allEmployees = context
                .Employees
                .OrderBy(x => x.EmployeeId)
                .ToArray();

            foreach (var e in allEmployees)
            {
                output.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
            }

            return output.ToString().Trim();
        }
        //Propblem 04
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var allEmployees = context
                .Employees
                .OrderBy(x => x.FirstName)
                .Where(e => e.Salary > 50000)
                .Select(x => new
                {
                    x.FirstName,
                    x.Salary
                })
                .ToArray();

            foreach (var e in allEmployees)
            {
                output.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }

            return output.ToString().Trim();
        }
        //Propblem 05
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var allEmployees = context
                .Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    DepartmentName = x.Department.Name,
                    x.Salary
                })
                .OrderBy(x => x.Salary)
                .ThenByDescending(x => x.FirstName)
                .ToArray();

            foreach (var e in allEmployees)
            {
                output.AppendLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:f2}");
            }

            return output.ToString().Trim();
        }
        //Propblem 06
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            Address newAddress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };
            context.Addresses.Add(newAddress);

            Employee nakov = context.Employees.First(e => e.LastName == "Nakov");

            nakov.Address = newAddress;
            context.SaveChanges();

            var addressTexts = context
                .Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => new
                {
                    e.Address.AddressText
                })
                .ToArray();

            foreach (var e in addressTexts)
            {
                output.AppendLine($"{e.AddressText}");
            }

            return output.ToString().TrimEnd();
        }
        //Propblem 07
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var employeesWithProjects = context.Employees
                .Where(e => e.EmployeesProjects.Any(
                    ep => ep.Project.StartDate.Year >= 2001 &&
                    ep.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFName = e.Manager.FirstName,
                    ManagerLName = e.Manager.LastName,
                    AllProjects = e.EmployeesProjects
                    .Select (ep => new
                    {
                        ProjectName = ep.Project.Name,
                        StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt"),
                        EndDate = ep.Project.EndDate.HasValue ? 
                        ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt")
                        : "not finished"
                    })
                    .ToArray()
                })
                .ToArray();

            foreach (var e in employeesWithProjects)
            {
                output.AppendLine(
                    $"{e.FirstName} {e.LastName} - Manager: {e.ManagerFName} {e.ManagerLName}");

                foreach (var p in e.AllProjects)
                {
                    output.AppendLine(
                        $"--{p.ProjectName} - {p.StartDate} - {p.EndDate}");
                }
            }

            return output.ToString();
        }
        //Propblem 08
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var addresses = context
                .Addresses
                .OrderByDescending(a => a.Employees.Count)
                .ThenBy(a => a.Town.Name)
                .Take(10)
                .Select(a => new
                {
                    a.AddressText,
                    TownName = a.Town.Name,
                    EmployeesCount = a.Employees.Count
                })
                .ToArray();

            foreach (var a in addresses)
            {
                output.AppendLine($"{a.AddressText}, {a.TownName} - {a.EmployeesCount} employees");
            }

            return output.ToString().Trim();
        }
        //Propblem 09
        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var emp147 = context
                .Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    AllProjects = e.EmployeesProjects
                    .Select(ep => new
                    {
                        ProjectName = ep.Project.Name
                    })
                    .ToArray()
                })
                .ToArray();

            var currEmp = emp147.FirstOrDefault();

            output.AppendLine($"{currEmp.FirstName} {currEmp.LastName} - {currEmp.JobTitle}");
            foreach (var p in currEmp.AllProjects.OrderBy(ep => ep.ProjectName))
            {
                output.AppendLine($"{p.ProjectName}");
            }

            return output.ToString().Trim();
        }
        //Problem 10
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var departments = context
                .Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    d.Name,
                    d.Manager.FirstName,
                    d.Manager.LastName,
                    AllEmployees = d.Employees
                    .Select(e => new
                    {
                        EmpFN = e.FirstName,
                        EmpLN = e.LastName,
                        EmpJT = e.JobTitle
                    })
                    .OrderBy(e => e.EmpFN)
                    .ThenBy(e => e.EmpLN)
                    .ToArray()
                })
                .ToArray();

            foreach (var d in departments)
            {
                output.AppendLine(
                    $"{d.Name} - {d.FirstName}  {d.LastName}");
                foreach (var e in d.AllEmployees)
                {
                    output.AppendLine(
                        $"{e.EmpFN} {e.EmpLN} - {e.EmpJT}");
                }
            }

            return output.ToString().Trim();
        }
        //Problem 11
        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var projects = context
                .Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt")
                })
                .OrderBy(p => p.Name)
                .ToArray();

            foreach (var p in projects)
            {
                output.AppendLine(p.Name);
                output.AppendLine(p.Description);
                output.AppendLine(p.StartDate);
            }

            return output.ToString().Trim();
        }
        //Problem 12
        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            Employee[] employees = context
                .Employees
                .Where(e => e.Department.Name == "Engineering" ||
                            e.Department.Name == "Tool Design" ||
                            e.Department.Name == "Marketing" ||
                            e.Department.Name == "Information Services")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToArray();
            
            foreach (var e in employees)
            {
                e.Salary *= 1.12m;
            }
            context.SaveChanges();


            foreach (var e in employees)
            {
                output.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:F2})");
            }

            return output.ToString().TrimEnd();
        }
        //Problem13
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var employees = context
                .Employees
                .Where(e => e.FirstName.StartsWith("Sa"))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToArray();

            foreach (var e in employees)
            {
                output.AppendLine(
                    $"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})");
            }

            return output.ToString().Trim();
        }
        //Problem 14
        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            Project projectToDelete = context
                .Projects
                .Find(2);

            EmployeeProject[] refferedEmployees = context
                .EmployeesProjects
                .Where(ep => ep.ProjectId == projectToDelete.ProjectId)
                .ToArray();
            context.EmployeesProjects.RemoveRange(refferedEmployees);
            context.Projects.Remove(projectToDelete);
            context.SaveChanges();

            string[] projectNames = context
                .Projects
                .Take(10)
                .Select(p => p.Name)
                .ToArray();

            foreach (var p in projectNames)
            {
                output.AppendLine(p);
            }

            return output.ToString().TrimEnd();
        }
        //Problem 15
        public static string RemoveTown(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            Employee[] employees = context
                .Employees
                .Where(e => e.Address.Town.Name == "Seattle")
                .ToArray();

            foreach (var e in employees)
            {
                e.AddressId = null;
            }
            context.SaveChanges();

            Address[] addrRemove = context
                .Addresses
                .Where(a => a.Town.Name == "Seattle")
                .ToArray();

            context.Addresses.RemoveRange(addrRemove);
            context.SaveChanges();

            Town townToRemove = context
                .Towns
                .First(t => t.Name == "Seattle");

            context.Towns.Remove(townToRemove);
            context.SaveChanges();

            output.AppendLine($"{addrRemove.Count()} addresses in Seattle were deleted");

            return output.ToString().TrimEnd();
        }
    }

}

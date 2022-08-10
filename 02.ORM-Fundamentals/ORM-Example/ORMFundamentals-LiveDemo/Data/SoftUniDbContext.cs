namespace ORMFundamentals_LiveDemo.Data
{
    using Microsoft.EntityFrameworkCore;

    using Models;

    public class SoftUniDbContext : DbContext
    {
        //Test cases
        public SoftUniDbContext()
        {
            
        }

        //Deploy -> Run DbContext(open Connection to SQL Server) from Application Builder
        //Services -> RegisterDbContext()
        public SoftUniDbContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
            
        }

        //Table Employees
        public DbSet<Employee> Employees { get; set; }

        //Table Departments
        public DbSet<Department> Departments { get; set; }

        //Configure the Connection to the SQL Server
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Default configuration
            base.OnConfiguring(optionsBuilder);

            //If the optionsBuilder is not configured (no ConnectionString)
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Config.ConnectionString);
            }
        }
    }
}

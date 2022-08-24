using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {
        }

        public StudentSystemContext(DbContextOptions options) 
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Homework> HomeworkSubmissions { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }

        //Establish a connection to SQL server
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            //True -> if connection string is already set
            //False -> if connection string isn't set
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Config.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Composite PK of mapping entity
            modelBuilder
                .Entity<StudentCourse>(e =>
                {
                    //Configuration for the curr entity (PlayerStatistic)
                    //Define PK
                    e.HasKey(ps => new { ps.StudentId, ps.CourseId });
                });
        }
    }
}

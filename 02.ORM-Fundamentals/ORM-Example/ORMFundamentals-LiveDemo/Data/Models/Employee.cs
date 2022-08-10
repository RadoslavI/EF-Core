// ReSharper disable InconsistentNaming
namespace ORMFundamentals_LiveDemo.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Employee
    {
        public Employee()
        {
            
        }

        [Key] //Primary key
        public int EmployeeID { get; set; }

        [Required] //Not Null
        [MaxLength(50)] //Max length of the VARCHAR
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        //Nullable Type (string) + w/o [Required] Attribute = Nullable VarChar(50)
        [MaxLength(50)]
        public string MiddleName { get; set; }

        [Required]
        [MaxLength(50)]
        public string JobTitle { get; set; }

        //One Relation
        [ForeignKey(nameof(Department))]
        public int DepartmentID { get; set; } //FK -> Absolute must, Defines Relation
        public virtual Department Department { get; set; } //Navigational Property, Not a Must, Very useful
    }
}

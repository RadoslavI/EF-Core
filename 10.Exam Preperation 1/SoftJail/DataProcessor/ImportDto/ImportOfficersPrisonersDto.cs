using SoftJail.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Officer")]
    public class ImportOfficersPrisonersDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        [XmlElement("Name")]
        public string FullName { get; set; }

        [XmlElement("Money")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Salary { get; set; }

        [Required]
        [XmlElement(nameof(Position))]
        public string Position { get; set; }

        [Required]
        [XmlElement(nameof(Weapon))]
        public string Weapon { get; set; }

        [XmlElement(nameof(DepartmentId))]
        public int DepartmentId { get; set; }

        [XmlArray("Prisoners")]
        public ImportPrisonerOfficerDto[] Prisoners { get; set; }
    }
}

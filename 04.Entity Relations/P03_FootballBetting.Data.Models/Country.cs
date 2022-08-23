using P03_FootballBetting.Data.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P03_FootballBetting.Data.Models
{
    public class Country
    {
        public Country()
        {
            Towns = new HashSet<Town>();
        }

        [Key]
        public int CountryId { get; set; }

        [Required]
        [MaxLength(GlobalConstants.CountryNameMaxLength)]
        public string Name { get; set; }

        //[InverseProperty(nameof(Town.Country))] WE DONT NEED IT WHEN THERE IS ONLY ONE!!!
        public virtual ICollection<Town> Towns { get; set; }
    }
}

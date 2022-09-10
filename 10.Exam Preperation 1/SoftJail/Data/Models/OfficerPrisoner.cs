using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SoftJail.Data.Models
{
    //Mapping Class
    public class OfficerPrisoner
    {
        [ForeignKey(nameof(Prisoner))]
        public int PrisonerId { get; set; }
        public Prisoner Prisoner { get; set; }

        [ForeignKey(nameof(Officer))]
        public int OfficerId { get; set; }
        public Officer Officer { get; set; }
    }
}

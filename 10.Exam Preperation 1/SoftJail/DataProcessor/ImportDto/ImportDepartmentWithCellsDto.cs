using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportDepartmentWithCellsDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(25)]
        [JsonProperty(nameof(Name))]
        public string Name { get; set; }

        [JsonProperty(nameof(Cells))]
        public ImportDepartmentCellDto[] Cells { get; set; }
    }
}

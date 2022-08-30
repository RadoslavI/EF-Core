using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO.Export
{
    [JsonObject]
    public class ExportPartsInfoDto
    {
        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}

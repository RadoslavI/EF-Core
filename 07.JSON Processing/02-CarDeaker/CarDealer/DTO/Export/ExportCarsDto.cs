using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO.Export
{
    [JsonObject]
    public class ExportCarsDto
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public long TravelledDistance { get; set; }

        [JsonProperty("parts")]
        public ExportPartsInfoDto Parts { get; set; }
    }
}

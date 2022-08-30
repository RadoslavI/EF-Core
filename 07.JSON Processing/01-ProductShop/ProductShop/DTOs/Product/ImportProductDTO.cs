using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductShop.DTOs.Product
{
    [JsonObject]
    public class ImportProductDTO
    {
        [JsonProperty("Name")]
        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [JsonProperty("Price")]
        public decimal Price { get; set; }

        [JsonProperty("SellerId")]
        public int SellerId { get; set; }

        [JsonProperty("BuyerId")]
        public int? BuyerId { get; set; }
    }
}

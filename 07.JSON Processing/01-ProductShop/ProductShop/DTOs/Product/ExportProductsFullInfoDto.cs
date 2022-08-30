using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductShop.DTOs.Product
{
    [JsonObject]
    public class ExportProductsFullInfoDto
    {
        [JsonProperty("count")]
        public int ProductsCount 
            => SoldProducts.Any() ? SoldProducts.Length : 0;

        [JsonProperty("products")]
        public ExportSoldProductShortInfoDto[] SoldProducts { get; set; }
    }
}

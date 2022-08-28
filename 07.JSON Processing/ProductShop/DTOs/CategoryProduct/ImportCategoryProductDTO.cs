using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.DTOs.Category_Product
{
    [JsonObject]
    public class ImportCategoryProductDTO
    {
        [JsonProperty("CategoryId")]
        public int CategoryId { get; set; }

        [JsonProperty("ProductId")]
        public int? ProductId { get; set; }
    }
}

using Newtonsoft.Json;
using ProductShop.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.DTOs.User
{
    [JsonObject]
    public class ExportUsersWithOneDTO
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("soldProducts")]
        public ExportUserSoldProductsDTO[] SoldProducts { get; set; }
    }
}

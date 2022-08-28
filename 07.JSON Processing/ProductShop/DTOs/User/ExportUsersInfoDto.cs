using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.DTOs.User
{
    [JsonObject]
    public class ExportUsersInfoDto
    {
        [JsonProperty("usersCount")]
        public int UsersCount 
            => Users.Length;

        [JsonProperty("users")]
        public ExportUsersWithFullProductInfoDto[] Users { get; set; }
    }
}

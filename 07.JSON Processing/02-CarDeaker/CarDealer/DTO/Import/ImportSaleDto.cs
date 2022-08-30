using Newtonsoft.Json;

namespace CarDealer.DTO.Import
{
    [JsonObject]
    public class ImportSaleDto
    {
        [JsonProperty("carId")]
        public int CarId { get; set; }

        [JsonProperty("customerId")]
        public int CustomerId { get; set; }

        [JsonProperty("discount")]
        public decimal Discount { get; set; }
    }
}

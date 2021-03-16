using Newtonsoft.Json;

namespace CarDealer.DTO
{
    public class CustomerTotalSalesDTO
    {
        [JsonProperty("fullName")]
        public string Name { get; set; }

        [JsonProperty("boughtCars")]
        public int CarsBought { get; set; }

        [JsonProperty("spentMoney")]
        public decimal SpentMoney { get; set; }
    }
}

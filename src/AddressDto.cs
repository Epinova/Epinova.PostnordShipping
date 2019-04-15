using Newtonsoft.Json;

namespace Epinova.PostnordShipping
{
    public class AddressDto
    {
        public string CountryCode { get; set; }
        public string PostalCode { get; set; }

        [JsonProperty("city")]
        public string PostalPlace { get; set; }

        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
    }
}
using Newtonsoft.Json;

namespace Epinova.PostnordShipping
{
    internal class CoordinateDto
    {
        public float Easting { get; set; }

        [JsonProperty("srId")]
        public string Id { get; set; }

        public float Northing { get; set; }
    }
}

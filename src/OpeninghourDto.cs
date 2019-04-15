using Newtonsoft.Json;

namespace Epinova.PostnordShipping
{
    internal class OpeningHourDto
    {
        public string Day { get; set; }

        [JsonProperty("from1")]
        public string From { get; set; }

        [JsonProperty("to1")]
        public string To { get; set; }
    }
}
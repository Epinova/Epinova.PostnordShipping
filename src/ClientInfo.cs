using System;

namespace Epinova.PostnordShipping
{
    public class ClientInfo
    {
        public string ApiKey { get; set; }
        public TimeSpan CacheTimeout { get; set; } = TimeSpan.FromDays(2);
        public CountryCode Country { get; set; }
        public string FilePath { get; set; }
    }
}
using System;
using EPiServer;
using EPiServer.Framework.Cache;
using EPiServer.Logging;

namespace Epinova.PostnordShipping
{
    internal class CacheHelper : ICacheHelper
    {
        private readonly ILogger _log;

        public CacheHelper(ILogger log)
        {
            _log = log;
        }

        public T Get<T>(string key) where T : class
        {
            return CacheManager.Get(key) as T;
        }

        public void Insert(string key, object value, TimeSpan timeToLive)
        {
            Insert(key, value, new CacheEvictionPolicy(timeToLive, CacheTimeoutType.Absolute));
        }


        public void Insert(string key, object value, CacheEvictionPolicy evictionPolicy)
        {
            if (String.IsNullOrWhiteSpace(key))
                return;

            _log.Debug($"Key: {key}, Item: {value}, CacheKey: {evictionPolicy?.CacheKeys}, Type: {evictionPolicy?.TimeoutType}, Seconds: {evictionPolicy?.Expiration.Duration().TotalSeconds}");

            CacheManager.Insert(key, value, evictionPolicy);
        }
    }
}
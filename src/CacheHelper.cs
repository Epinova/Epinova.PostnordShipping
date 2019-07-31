using System;
using EPiServer.Framework.Cache;
using EPiServer.Logging;

namespace Epinova.PostnordShipping
{
    public class CacheHelper : ICacheHelper
    {
        private readonly ISynchronizedObjectInstanceCache _cacheManager;
        private readonly ILogger _log;

        public CacheHelper(ILogger log, ISynchronizedObjectInstanceCache cacheManager)
        {
            _log = log;
            _cacheManager = cacheManager;
        }

        public T Get<T>(string key) where T : class
        {
            return _cacheManager.Get(key) as T;
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

            _cacheManager.Insert(key, value, evictionPolicy);
        }
    }
}